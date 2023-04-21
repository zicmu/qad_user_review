using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QAD_User_Review.Data;
using QAD_User_Review.Models;

namespace QAD_User_Review.ViewModels
{
    public class MenuDetailViewModel
    {
        public IList<MenuDetail> MenuDetails { set; get; }
        public string selectedGroup { set; get; }

    }


}
