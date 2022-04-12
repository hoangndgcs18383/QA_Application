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
        public int CountThumb { get; set; } = 0;
        public int CountThumbUp { get; set; } = 0;
        public int CountThumbDown { get; set; } = 0;
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
        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Thumb> Thumbs { get; set; }

    }
}
