using Microsoft.AspNetCore.Mvc.Rendering;

namespace QAD_User_Review.ViewModels
{
    public class ReviewListMainViewModel
    {
        public IList<ReviewItemViewModel> ReviewItems { get; set; } = new List<ReviewItemViewModel>();
        public IEnumerable<SelectListItem> Statuses { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Employees { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Plants { get; set; } = Enumerable.Empty<SelectListItem>();
        public string ReviewerName { get; set; } = string.Empty;
        public string ActivePeriodName { get; set; } = string.Empty;
        public string? SelectedEmployee { get; set; }
        public string? SelectedStatus { get; set; }
        public string? SelectedPlant { get; set; }
    }
}
