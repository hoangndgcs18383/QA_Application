using System.ComponentModel.DataAnnotations;

namespace QA_Application.Models
{
    public class SpecialTag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Special Tag Name")]
        public string SpecialTagName { get; set; }
    }
}
