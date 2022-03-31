namespace QA_Application.Models
{
    public class Pager
    {
        public int currentPage { get; set; }
        public int countPage { get; set; }
        public Func<int?, string> generateUrl { get; set; }
    }
}
