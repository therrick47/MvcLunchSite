using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcLunchSite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }
        public int? FirstChoice { get; set; }
        public int? SecondChoice { get; set; }
        public int? ThirdChoice { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<MvcLunchSite.Models.Restaurant> Restaurants { get; set; }

        public System.Data.Entity.DbSet<MvcLunchSite.Models.Menu> Menus { get; set; }

        public System.Data.Entity.DbSet<MvcLunchSite.Models.MenuItem> MenuItems { get; set; }

        public System.Data.Entity.DbSet<MvcLunchSite.Models.VoteRecord> VoteRecords { get; set; }
    }
}