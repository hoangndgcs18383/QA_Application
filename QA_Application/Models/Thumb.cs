using System.ComponentModel.DataAnnotations.Schema;

namespace QA_Application.Models
{
    public class Thumb
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public ApplicationUser Author { get; set; }
        public string toggle { get; set; } = null;
        public bool isThumb { get; set; } = false;
    }
}
