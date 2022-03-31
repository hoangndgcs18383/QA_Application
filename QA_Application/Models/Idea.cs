using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QA_Application.Models
{
    public class Idea
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Display(Name = "File .pdf")]
        public string? FileSubmit { get; set; }
        [Required]
        [Display(Name = "Create At")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastUpdateDate { get; set; }
        public bool? isApproved { get; set; }

        public bool? isLocked { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [Display(Name = "Special Tag")]
        public int SpecialTagId { get; set; }
        [ForeignKey("SpecialTagId")]
        public SpecialTag SpecialTag { get; set; }
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public ApplicationUser Author { get; set; }
        public virtual IEnumerable<Comment> Comments { get; set; }

    }
}
