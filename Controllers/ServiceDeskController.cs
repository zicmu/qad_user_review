using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting.Internal;
using QAD_User_Review.ViewModels;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace QAD_User_Review.Controllers
{

    public class ServiceDeskController : Controller
    {
        private IHostingEnvironment Environment;
        public ServiceDeskController(IHostingEnvironment _environment)
        {
            Environment = _environment;
        }


        public ActionResult Index()
        {
            string[] filepaths = Directory.GetFiles(Path.Combine(this.Environment.WebRootPath, "Files/ServiceDesk/"));
            List<FileModel> list = new List<FileModel>();
            foreach (string filepath in filepaths)
            {
                list.Add(new FileModel { FileName = Path.GetFileName(filepath) });
            }
            return View(list);
        }

        public FileResult DownloadFile(string fileName)
        {
            string path = Path.Combine(this.Environment.WebRootPath, "Files/ServiceDesk/") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}
