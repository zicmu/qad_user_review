using Microsoft.AspNetCore.Mvc;
using QAD_User_Review.ViewModels;

namespace QAD_User_Review.Controllers
{
    public class ItDocsController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public ItDocsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public ActionResult Index()
        {
            string[] filepaths = Directory.GetFiles(Path.Combine(_environment.WebRootPath, "Files"));
            var list = filepaths
                .Select(fp => new FileModel { FileName = Path.GetFileName(fp) })
                .ToList();
            return View(list);
        }

        public FileResult DownloadFile(string fileName)
        {
            string path = Path.Combine(_environment.WebRootPath, "Files", fileName);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}
