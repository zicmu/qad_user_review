using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Models;
using QAD_User_Review.Data;
using QAD_User_Review.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.NetworkInformation;
using System.Web;
using System.Diagnostics;
using System.Net.Mail;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.Query.Internal;
//using AspNetCore;

namespace QAD_User_Review.Controllers

{
    public class ReviewListController : Controller
    {
        private readonly UserReviewContext _context;
        private object tReviewList;

        public object JsonRequestBehavior { get; private set; }

        public ReviewListController(UserReviewContext context)
        {
            _context = context;
        }

                public async Task<IActionResult> Index()
                {

                    // Set the current cursor to the wait cursor.
                    string managerUserID = User.Identity.Name.ToString().ToLower().Split('\\')[1];

                    managerUserID = ReturnManagerUserID(managerUserID);

                    int isManager = _context.Managers.Where(d => d.UserId.ToLower() == managerUserID).Count();

                    if ((isManager == null) || (isManager == 0))
                    {
                        TempData["SuccessMessage"] = "You are not authorized to access reivew list.\n\r Please contact administrator.";
                        return RedirectToAction("Index", "Home");

                    }


                    List<ReviewList> reviewList =  GetReviewList(managerUserID).ToList();            
                    var employeeList = _context.Users.Where(d => d.ManagerUserName.ToLower() == managerUserID & d.Valid != "OLD").ToList();
                    var managerList = _context.Managers.Where(d => d.UserId.ToLower() == managerUserID).ToList();
                    var plantList = new SelectList(_context.Users.Where(d => d.ManagerUserName.ToLower() == managerUserID).ToList(), "Plant", "Plant").ToList().DistinctBy(x => x.Text);
                    //var plantList = _plantList.DistinctBy(x => x.Text);


                    var viewModel = new ViewModels.ReviewListMainViewModel
                    {
                        ReviewLists = reviewList,
                        Employees = employeeList,
                        Managers = managerList,
                        Plants = plantList,
                        Decisions = new List<SelectListItem>
                                        {
                                        new SelectListItem {Text ="Disable",Value="Disable"},
                                        new SelectListItem {Text ="Approved",Value="Approved"},
                                        new SelectListItem {Text ="Open",Value="Open"}
                                        }

                    };

       
                    return View("ReviewList",  viewModel);
                    //return View("ReviewList", await tReviewList);


                }
        
        public IEnumerable<ReviewList> GetReviewList (string ManagerUserID) 
            {

                       
            IQueryable<ReviewList> query = _context.ReviewLists.Where(d => d.ManagerUsername.ToLower().Contains(ManagerUserID));
                                 
            return query;

            }
        [HttpPost]
        public async Task<IActionResult> Save(ReviewListMainViewModel reviewList)
        {
           
            string managerUserID = User.Identity.Name.ToString().ToLower().Split('\\')[1];
           
            int cntChanges = 0;

            var tReviewLists = _context.ReviewLists.Where(d => reviewList.ReviewLists.Select(x => x.Id).Contains(d.Id));

            foreach (var tReviewList in tReviewLists)
            {
                var item = reviewList.ReviewLists.First(d => d.Id == tReviewList.Id);

                if (tReviewList.Decision != item.Decision || tReviewList.Note != item.Note)
                {
                    tReviewList.Decision = item.Decision;
                    tReviewList.Note = item.Note;
                    tReviewList.ChangedBy = managerUserID;
                    tReviewList.ChangedOn = DateTime.Now;
                    cntChanges = cntChanges + 1;
                }
            }

            _context.UpdateRange(tReviewLists);
            await _context.SaveChangesAsync();

            // add pop up message that save was sucesfully done or failure
            string passMessage = "Data has been sucessfully saved!\r\n";
            if (cntChanges == 0)
            {
                passMessage = passMessage + "No changes made.";

            }
            else
            {
                passMessage = passMessage + "Number of made changes is " + cntChanges + ".";
            }
        
            TempData["SuccessMessage"] = passMessage;

            return RedirectToAction("Index", "ReviewList");
        }

    

        public async Task<IActionResult> MenuDetails(string? selectedGroup)
        {

          
            selectedGroup = selectedGroup.ToUpper();

            IQueryable<Models.MenuDetail> query = _context.MenuDetails;

            query = query.Where(d => d.UserGroupFullName.ToUpper() == selectedGroup);

            var listMenuDetails = await query.ToListAsync();

            var viewModel = new ViewModels.MenuDetailViewModel
            {
                MenuDetails = listMenuDetails,
                selectedGroup = selectedGroup


            };
            return PartialView("MenuDetails", viewModel);

        }

        public string ReturnManagerUserID (string ManagerUserID)
        {

            if ( ManagerUserID=="edabici" || ManagerUserID=="epetkoz")
            {
                ManagerUserID = "earesm";

            }
            return ManagerUserID;
        }

       

      
    }
   


}
