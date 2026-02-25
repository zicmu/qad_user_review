using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;

namespace QAD_User_Review.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {

           
            _logger = logger;
        }

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult New()

        {
                                    
            //  string managerUserID = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string managerUserID = User.Identity.Name.ToString().ToLower().Split('\\')[1];

            TempData["Result"] = managerUserID;
            return View();
        }

        

        


          }

    
}







