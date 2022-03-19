using System.ComponentModel.DataAnnotations;

namespace QA_Application.Areas.Manager.ViewModels
{
    public class UserRoleViewModel
    {
        [Required]
        [Display(Name = "User")]
        public string RoleId { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string UserId { get; set; }
    }
}
