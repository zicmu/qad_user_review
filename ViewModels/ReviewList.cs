using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;
using QAD_User_Review.Models;

namespace QAD_User_Review.ViewModels
{
    public class ReviewListMainViewModel
    {
        public IList<ReviewList> ReviewLists { set; get; }
        public IEnumerable<SelectListItem> Decisions { set; get; }
        public IEnumerable<User> Employees { set; get; }
        public IEnumerable<Manager> Managers { set; get; }
        public IEnumerable<SelectListItem> Plants { set; get; }
        public string selectedEmployee { set; get; }
        public string selectedDecision { set; get; }
        public string selectedPlant { set; get; }
    }

    
}
