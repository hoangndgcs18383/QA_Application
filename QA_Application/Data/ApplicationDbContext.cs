using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QA_Application.Models;

namespace QA_Application.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SpecialTag> SpecialTags { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}