using System.Data;
using System.Diagnostics;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;
using QAD_User_Review.ViewModels;

namespace QAD_User_Review.Services
{
    public class DataIngestionService : IDataIngestionService
    {
        private readonly UserReviewContext _context;
        private readonly ILogger<DataIngestionService> _logger;
        private readonly string _debugLogPath;

        private record FileSpec(string TableName, string Label, string[] Columns);

        private static readonly FileSpec[] AllSpecs =
        {
            new("Stg_Raw_SAP_OrgHier",   "SAP Org Hierarchy",     new[] { "UserId", "FullName", "ManagerName", "ManagerEmail", "ManagerUserId" }),
            new("Stg_Raw_SAP_Rollen",     "SAP Roles",             new[] { "Benutzer", "VollstaendigerName", "Rolle", "Typ", "Zuordnung", "HROrg", "RolleBeschreibung", "Abteilung", "Startdatum", "Enddatum", "Manager" }),
            new("Stg_Raw_QAD_Users2007",  "QAD 2007 Users",        new[] { "UserID", "UserName" }),
            new("Stg_Raw_QAD_Users2008",  "QAD 2008 Users",        new[] { "UserID", "UserName" }),
            new("Stg_Raw_QAD_DEFRGB",     "QAD 2007 Roles",        new[] { "GroupCode", "GroupDescription", "Domain", "UserID", "UserName" }),
            new("Stg_Raw_QAD_ITRS",       "QAD 2008 Roles",        new[] { "GroupCode", "GroupDescription", "Domain", "UserID", "UserName" }),
            new("Stg_Raw_QAD_OrgHier",    "QAD Org Hierarchy",     new[] { "Employee", "Username", "Email", "Plant", "Country", "Manager", "Valid", "ManagerUserName" }),
        };

        private static readonly Dictionary<string, int> FileKeyMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SapOrgHier"]   = 0,
            ["SapRollen"]    = 1,
            ["QadUsers2007"] = 2,
            ["QadUsers2008"] = 3,
            ["QadDefrgb"]    = 4,
            ["QadItrs"]      = 5,
            ["QadOrgHier"]   = 6,
        };

        public DataIngestionService(UserReviewContext context, ILogger<DataIngestionService> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _debugLogPath = System.IO.Path.Combine(env.ContentRootPath ?? ".", "debug-e6a1c2.log");
        }

        // #region agent log
        private void DebugLog(string location, string message, object? data = null, string? hypothesisId = null)
        {
            try
            {
                var payload = new Dictionary<string, object?>
                {
                    ["sessionId"] = "e6a1c2",
                    ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ["location"] = location,
                    ["message"] = message
                };
                if (data != null) payload["data"] = data;
                if (!string.IsNullOrEmpty(hypothesisId)) payload["hypothesisId"] = hypothesisId;
                var line = JsonSerializer.Serialize(payload) + Environment.NewLine;
                System.IO.File.AppendAllText(_debugLogPath, line);
            }
            catch { /* avoid breaking app */ }
        }
        // #endregion

        public async Task<IngestionResult> ProcessFilesAsync(IngestionFileSet files)
        {
            var sw = Stopwatch.StartNew();
            var result = new IngestionResult { Success = true };

            var uploads = new (IFormFile? File, FileSpec Spec)[]
            {
                (files.SapOrgHier,   AllSpecs[0]),
                (files.SapRollen,    AllSpecs[1]),
                (files.QadUsers2007, AllSpecs[2]),
                (files.QadUsers2008, AllSpecs[3]),
                (files.QadDefrgb,    AllSpecs[4]),
                (files.QadItrs,      AllSpecs[5]),
                (files.QadOrgHier,   AllSpecs[6]),
            };

            bool anyFile = uploads.Any(u => u.File != null);
            if (!anyFile)
            {
                result.Success = false;
                result.Messages.Add("No files were selected for upload.");
                return result;
            }

            string connStr = _context.Database.GetConnectionString()!;

            foreach (var (file, spec) in uploads)
            {
                if (file == null || file.Length == 0)
                    continue;

                try
                {
                    int rows = await LoadFileAsync(file, spec, connStr);
                    result.RowCounts[spec.Label] = rows;
                    result.Messages.Add($"{spec.Label}: {rows:N0} rows loaded.");
                    _logger.LogInformation("Loaded {Rows} rows into {Table} from {File}",
                        rows, spec.TableName, file.FileName);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Messages.Add($"{spec.Label}: ERROR — {ex.Message}");
                    _logger.LogError(ex, "Failed to load {Table} from {File}",
                        spec.TableName, file.FileName);
                }
            }

            if (result.Success)
            {
                try
                {
                    await RunStoredProcedureAsync(connStr);
                    result.Messages.Add("ETL transform (sp_IngestSourceData) completed successfully.");
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Messages.Add($"ETL transform failed: {ex.Message}");
                    _logger.LogError(ex, "sp_IngestSourceData failed");
                }
            }

            sw.Stop();
            result.Duration = sw.Elapsed;
            return result;
        }

        public async Task<IngestionResult> RunTransformAsync(DateTime? lastUpdateDate = null)
        {
            var sw = Stopwatch.StartNew();
            var result = new IngestionResult { Success = true };

            try
            {
                string connStr = _context.Database.GetConnectionString()!;
                await RunStoredProcedureAsync(connStr);
                result.Messages.Add("ETL transform (sp_IngestSourceData) completed successfully.");

                if (lastUpdateDate.HasValue)
                {
                    await UpdateActiveReviewPeriodLastUpdateDateAsync(connStr, lastUpdateDate.Value);
                    result.Messages.Add($"Active review period LastUpdateDate set to {lastUpdateDate.Value:yyyy-MM-dd}.");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.Add($"ETL transform failed: {ex.Message}");
                _logger.LogError(ex, "sp_IngestSourceData failed");
            }

            sw.Stop();
            result.Duration = sw.Elapsed;
            return result;
        }

        public async Task<IngestionResult> LoadSingleFileAsync(IFormFile file, string fileKey)
        {
            var sw = Stopwatch.StartNew();
            var result = new IngestionResult { Success = true };

            if (!FileKeyMap.TryGetValue(fileKey, out int specIndex))
            {
                result.Success = false;
                result.Messages.Add($"Unknown file key: {fileKey}");
                return result;
            }

            var spec = AllSpecs[specIndex];
            string connStr = _context.Database.GetConnectionString()!;

            try
            {
                int rows = await LoadFileAsync(file, spec, connStr);
                result.RowCounts[spec.Label] = rows;
                result.Messages.Add($"{spec.Label}: {rows:N0} rows loaded.");
                _logger.LogInformation("Loaded {Rows} rows into {Table} from {File}",
                    rows, spec.TableName, file.FileName);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Messages.Add($"{spec.Label}: ERROR — {ex.Message}");
                _logger.LogError(ex, "Failed to load {Table} from {File}",
                    spec.TableName, file.FileName);
            }

            sw.Stop();
            result.Duration = sw.Elapsed;
            return result;
        }

        private async Task<int> LoadFileAsync(IFormFile file, FileSpec spec, string connStr)
        {
            // #region agent log
            if (spec.TableName == "Stg_Raw_QAD_DEFRGB")
                DebugLog("DataIngestionService.LoadFileAsync", "DEFRGB upload started", new Dictionary<string, object> { ["debugLogPath"] = _debugLogPath, ["fileName"] = file?.FileName ?? "" }, "A");
            // #endregion
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();

            var dataTable = BuildDataTable(worksheet, spec, file.FileName);

            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            using var trunc = conn.CreateCommand();
            trunc.CommandText = $"TRUNCATE TABLE [{spec.TableName}]";
            await trunc.ExecuteNonQueryAsync();

            using var bulk = new SqlBulkCopy(conn)
            {
                DestinationTableName = spec.TableName,
                BatchSize = 5000,
                BulkCopyTimeout = 120
            };

            foreach (DataColumn col in dataTable.Columns)
                bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

            await bulk.WriteToServerAsync(dataTable);

            return dataTable.Rows.Count;
        }

        private DataTable BuildDataTable(IXLWorksheet ws, FileSpec spec, string fileName)
        {
            var dt = new DataTable(spec.TableName);

            foreach (string col in spec.Columns)
                dt.Columns.Add(col, typeof(string));

            dt.Columns.Add("ETL_LoadDate", typeof(DateTime));
            dt.Columns.Add("ETL_FileName", typeof(string));

            var headerRow = ws.Row(1);
            int colCount = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            int[] colMap = ResolveColumnMap(headerRow, colCount, spec);

            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            var now = DateTime.UtcNow;

            for (int r = 2; r <= lastRow; r++)
            {
                var row = ws.Row(r);

                bool allEmpty = true;
                for (int c = 0; c < spec.Columns.Length; c++)
                {
                    if (colMap[c] > 0 && !row.Cell(colMap[c]).IsEmpty())
                    {
                        allEmpty = false;
                        break;
                    }
                }
                if (allEmpty) continue;

                var dr = dt.NewRow();
                for (int c = 0; c < spec.Columns.Length; c++)
                {
                    if (colMap[c] > 0)
                    {
                        var cell = row.Cell(colMap[c]);
                        var val = GetCellString(cell);
                        dr[c] = string.IsNullOrEmpty(val) ? DBNull.Value : val;
                    }
                    else
                    {
                        dr[c] = DBNull.Value;
                    }
                }
                dr["ETL_LoadDate"] = now;
                dr["ETL_FileName"] = fileName;
                // #region agent log
                if (spec.TableName == "Stg_Raw_QAD_DEFRGB" && r == 2)
                {
                    var groupCodeColIdx = colMap[0];
                    string? rawCellValue = null;
                    string? cellValueType = null;
                    string? cellValueOther = null;
                    if (groupCodeColIdx > 0)
                    {
                        var cell = row.Cell(groupCodeColIdx);
                        var strVal = cell.GetString();
                        rawCellValue = cell.IsEmpty() ? "(empty)" : (string.IsNullOrWhiteSpace(strVal) ? "(blank string)" : strVal);
                        try { cellValueType = cell.DataType.ToString(); } catch { }
                        try { var v = cell.GetValue<object>(); cellValueOther = v?.ToString(); } catch { }
                    }
                    DebugLog("DataIngestionService.BuildDataTable", "DEFRGB first row GroupCode",
                        new Dictionary<string, object?> {
                            ["groupCodeExcelCol"] = groupCodeColIdx,
                            ["valueWritten"] = dr["GroupCode"]?.ToString() ?? "(null)",
                            ["rawCellValue"] = rawCellValue,
                            ["cellValueType"] = cellValueType,
                            ["cellValueOther"] = cellValueOther
                        },
                        "D");
                }
                // #endregion
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// Tries header-name matching first (case-insensitive, whitespace-normalized).
        /// Falls back to positional mapping if fewer than half the headers match by name.
        /// </summary>
        private int[] ResolveColumnMap(IXLRow headerRow, int colCount, FileSpec spec)
        {
            int[] map = new int[spec.Columns.Length];

            var headerIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int c = 1; c <= colCount; c++)
            {
                string hdr = Normalize(GetCellString(headerRow.Cell(c)));
                if (!string.IsNullOrEmpty(hdr) && !headerIndex.ContainsKey(hdr))
                    headerIndex[hdr] = c;
            }

            int matched = 0;
            for (int i = 0; i < spec.Columns.Length; i++)
            {
                if (headerIndex.TryGetValue(Normalize(spec.Columns[i]), out int idx))
                {
                    map[i] = idx;
                    matched++;
                }
            }

            // Try aliases for any unmatched columns (e.g. GroupCode <- "Group", "Role", "Code")
            if (ColumnAliases != null)
            {
                for (int i = 0; i < spec.Columns.Length; i++)
                {
                    if (map[i] != 0) continue;
                    if (!ColumnAliases.TryGetValue(spec.Columns[i], out var aliases)) continue;
                    foreach (var alias in aliases)
                    {
                        var normalized = Normalize(alias);
                        if (string.IsNullOrEmpty(normalized)) continue;
                        if (headerIndex.TryGetValue(normalized, out int idx))
                        {
                            map[i] = idx;
                            matched++;
                            break;
                        }
                    }
                }
            }

            // #region agent log
            if (spec.TableName == "Stg_Raw_QAD_DEFRGB")
            {
                var headers = new List<Dictionary<string, object>>();
                for (int c = 1; c <= colCount; c++)
                {
                    var raw = headerRow.Cell(c).GetString();
                    headers.Add(new Dictionary<string, object> { ["excelCol"] = c, ["raw"] = raw ?? "", ["normalized"] = Normalize(raw ?? "") });
                }
                var specMap = new List<Dictionary<string, object>>();
                for (int i = 0; i < spec.Columns.Length; i++)
                    specMap.Add(new Dictionary<string, object> { ["specCol"] = spec.Columns[i], ["excelCol"] = map[i] });
                DebugLog("DataIngestionService.ResolveColumnMap", "DEFRGB column resolution",
                    new Dictionary<string, object> {
                        ["colCount"] = colCount,
                        ["headers"] = headers,
                        ["specMap"] = specMap,
                        ["matched"] = matched,
                        ["total"] = spec.Columns.Length,
                        ["groupCodeExcelCol"] = map[0]
                    },
                    "A");
            }
            // #endregion

            if (matched >= spec.Columns.Length / 2)
            {
                for (int i = 0; i < spec.Columns.Length; i++)
                    if (map[i] == 0) map[i] = -1;

                // DEFRGB: ensure first column (Group) always maps to GroupCode if still unmapped
                if (spec.TableName == "Stg_Raw_QAD_DEFRGB" && (map[0] == 0 || map[0] == -1) && colCount >= 1)
                    map[0] = 1;

                // SAP Rollen: ensure common columns map by position if still unmapped (Benutzer, VollstaendigerName, HROrg, RolleBeschreibung)
                if (spec.TableName == "Stg_Raw_SAP_Rollen" && colCount >= 7)
                {
                    if (map[0] == 0 || map[0] == -1) map[0] = 1;   // Benutzer <- col 1
                    if (map[1] == 0 || map[1] == -1) map[1] = 2;   // VollstaendigerName <- col 2
                    if (map[5] == 0 || map[5] == -1) map[5] = 6;   // HROrg <- col 6
                    if (map[6] == 0 || map[6] == -1) map[6] = 7;   // RolleBeschreibung <- col 7
                }

                // #region agent log
                if (spec.TableName == "Stg_Raw_QAD_DEFRGB")
                    DebugLog("DataIngestionService.ResolveColumnMap", "DEFRGB used header matching", new Dictionary<string, object> { ["matched"] = matched, ["groupCodeExcelColAfter"] = map[0] }, "A");
                // #endregion
                return map;
            }

            _logger.LogWarning(
                "Header-name matching found only {Matched}/{Total} for {Table}; falling back to positional mapping",
                matched, spec.Columns.Length, spec.TableName);

            for (int i = 0; i < spec.Columns.Length; i++)
                map[i] = i + 1 <= colCount ? i + 1 : -1;

            // #region agent log
            if (spec.TableName == "Stg_Raw_QAD_DEFRGB")
                DebugLog("DataIngestionService.ResolveColumnMap", "DEFRGB used positional fallback", new Dictionary<string, object> { ["groupCodeExcelCol"] = map[0] }, "B");
            // #endregion
            return map;
        }

        private static string Normalize(string s) =>
            string.Concat(s.Where(c => !char.IsWhiteSpace(c)));

        /// <summary>Optional header aliases for column resolution (normalized). Tried when exact name does not match.
        /// DEFRGB_template.xlsx uses "Group" in the first column; that maps to GroupCode here.
        /// SAP Roles source may use Bentuzer, VollastaendigerName, HRorg, Rolle Beschreibung.</summary>
        private static readonly IReadOnlyDictionary<string, string[]> ColumnAliases = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["GroupCode"] = new[] { "Group", "Role", "Code" },
            ["Benutzer"] = new[] { "Bentuzer", "User", "Benutzer" },
            ["VollstaendigerName"] = new[] { "VollastaendigerName", "VollständigerName", "Vollstaendiger Name", "Vollständiger Name", "FullName" },
            ["HROrg"] = new[] { "HRorg", "HR Org", "HR-Org", "HROrg" },
            ["RolleBeschreibung"] = new[] { "Rolle Beschreibung", "Rollenbeschreibung", "RolleBeschreibung", "Role Description" },
        };

        /// <summary>Gets cell value as string; uses GetValue() when GetString() is empty so numeric group codes are read.</summary>
        private static string GetCellString(IXLCell cell)
        {
            if (cell.IsEmpty()) return string.Empty;
            var s = cell.GetString();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            try { return cell.GetValue<object>()?.ToString() ?? string.Empty; } catch { return string.Empty; }
        }

        private static async Task RunStoredProcedureAsync(string connStr)
        {
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "EXEC sp_IngestSourceData";
            cmd.CommandTimeout = 300;
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task UpdateActiveReviewPeriodLastUpdateDateAsync(string connStr, DateTime lastUpdateDate)
        {
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Dim_ReviewPeriod SET LastUpdateDate = @date WHERE IsActive = 1";
            cmd.Parameters.AddWithValue("@date", lastUpdateDate);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
