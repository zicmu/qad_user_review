namespace QAD_User_Review.ViewModels
{
    public class SharedReviewListViewModel
    {
        public IList<ReviewItemViewModel> ReviewItems { get; set; } = new List<ReviewItemViewModel>();
    }
}
