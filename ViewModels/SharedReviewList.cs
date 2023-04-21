using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;
using QAD_User_Review.Models;

namespace QAD_User_Review.ViewModels
{
    public class SharedReviewListViewModel
    {
        public IList<ReviewList> ReviewLists { set; get; }
       
    }


}
