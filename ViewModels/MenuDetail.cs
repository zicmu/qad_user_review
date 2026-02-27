using QAD_User_Review.Models;

namespace QAD_User_Review.ViewModels
{
    public class MenuDetailViewModel
    {
        public IList<DimMenu> MenuItems { get; set; } = new List<DimMenu>();
        public string SelectedRole { get; set; } = string.Empty;
    }
}
