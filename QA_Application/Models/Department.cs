using System.ComponentModel.DataAnnotations;

namespace QA_Application.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
    }
}
