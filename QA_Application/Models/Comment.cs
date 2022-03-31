using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QA_Application.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public ApplicationUser Author { get; set; }
    }
}
