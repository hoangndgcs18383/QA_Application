using QA_Application.Models;

namespace QA_Application.ViewModels
{
    public class CategoryViewModel
    {
        public Category Category;
        public virtual IEnumerable<Idea> Idea { get; set; }
    }
}
