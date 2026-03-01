using Microsoft.AspNetCore.Http;
using QAD_User_Review.ViewModels;

namespace QAD_User_Review.Services
{
    public interface IDataIngestionService
    {
        Task<IngestionResult> ProcessFilesAsync(IngestionFileSet files);
        Task<IngestionResult> RunTransformAsync(DateTime? lastUpdateDate = null);
        Task<IngestionResult> LoadSingleFileAsync(IFormFile file, string fileKey);
    }
}
