using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QA_Application.Models
{
    public class Idea
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [Display(Name = "Create At")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Display(Name = "Available")]
        public bool isAvailable { get; set; }

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

    }
}
