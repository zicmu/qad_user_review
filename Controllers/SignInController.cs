using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;
using QAD_User_Review.Models;

namespace QAD_User_Review.Controllers
{
    public class SignInController : Controller
   
       {

        private readonly UserReviewContext _context;
        private object tReviewList;

        public object JsonRequestBehavior { get; private set; }

        public SignInController(UserReviewContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            
            var Managers = _context.Managers.ToList();
            return View("SignIn", Managers);
        }
    }
}
