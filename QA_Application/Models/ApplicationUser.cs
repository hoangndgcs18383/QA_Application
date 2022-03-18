using Microsoft.AspNetCore.Identity;

namespace QA_Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
