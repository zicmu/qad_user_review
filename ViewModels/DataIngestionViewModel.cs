using Microsoft.AspNetCore.Http;

namespace QAD_User_Review.ViewModels
{
    public class IngestionFileSet
    {
        public IFormFile? SapOrgHier { get; set; }
        public IFormFile? SapRollen { get; set; }
        public IFormFile? QadUsers2007 { get; set; }
        public IFormFile? QadUsers2008 { get; set; }
        public IFormFile? QadDefrgb { get; set; }
        public IFormFile? QadItrs { get; set; }
        public IFormFile? QadOrgHier { get; set; }
    }

    public class IngestionResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; } = new();
        public Dictionary<string, int> RowCounts { get; set; } = new();
        public TimeSpan Duration { get; set; }
    }

    public class DataIngestionViewModel
    {
        public IngestionResult? LastResult { get; set; }

        /// <summary>Date when the source data was extracted; used as LastUpdateDate on the active review period.</summary>
        public DateTime? DataExtractedDate { get; set; }
    }

    /// <summary>Request body for RunTransform: optional last update date for the active review period.</summary>
    public class RunTransformRequest
    {
        /// <summary>ISO date string (yyyy-MM-dd) when the source data was extracted.</summary>
        public string? LastUpdateDate { get; set; }
    }
}
