using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QA_Application.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Closesure date")]
        public DateTime FirstDeadline { get; set; }
        [Required]
        [Display(Name = "Final closesure date")]
        public DateTime FinalDeadline { get; set; }
        [Display(Name = "Parent Category")]
        public int? ParentCategoryId { get; set; }
        [ForeignKey("ParentCategoryId")]
        public Category ParentCategory { set; get; }
        public int? IdeaId { get; set; }
        [ForeignKey("IdeaId")]
        public Idea Idea { get; set; }

        public ICollection<Category> CategoryChildren { get; set; }

    }
}
