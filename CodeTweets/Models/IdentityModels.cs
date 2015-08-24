using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace CodeTweets.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual List<ApplicationUser> followList { get; set; }

        public virtual List<ApplicationUser> blockedList { get; set; }

        public virtual List<UsersVotes> usersVotes { get; set; }

        public int theme_id { get; set; }

        public string user { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ExploreUsersViewModel
    {
        public string Id { get; set; }
        public string user { get; set; }
        public string UserName { get; set; }

        public string isFollowed { get; set; }
        public string isBlocked { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<CodePost> posts { get; set; }
        public DbSet<HashTagPost> hashTags { get; set; }
        public DbSet<HashTag> tags { get; set; }
        public DbSet<UsersVotes> votes { get; set; }
        public DbSet<BlockedUsers> blockedUsers { get; set; }
        public DbSet<UsersFollow> followedUsers { get; set; }

        public ApplicationDbContext()
            : base("ApplicationDbContext", throwIfV1Schema: false)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasMany(m => m.followList).WithMany();

            modelBuilder.Entity<ApplicationUser>().HasMany(m => m.blockedList).WithMany();

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}