using System.ComponentModel.DataAnnotations;

namespace QA_Application.Areas.Manager.ViewModels
{
    public class UserRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleId { get; set; }
        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; }
    }
}
