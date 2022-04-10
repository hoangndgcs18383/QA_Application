using QA_Application.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QA_Application.ViewModels
{
    public class ThumbViewModel
    {
        [Key]
        public int ThumbId { get; set; }
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public ApplicationUser Author { get; set; }
        public string toggle { get; set; }
        public int IdeaId { get; set; }
    }
}
