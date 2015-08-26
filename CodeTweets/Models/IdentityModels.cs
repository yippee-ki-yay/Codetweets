﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

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

    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        public object PasswordHash { get; private set; }

        protected override void Seed(ApplicationDbContext context)
        {
            var PasswordHash = new PasswordHasher();

            context.Users.Add(new ApplicationUser() { user = "sdfds", UserName="nesa@gmail.com", PasswordHash = PasswordHash.HashPassword("123456") });
            context.SaveChanges();
            base.Seed(context);
        }
    }

    public class ExploreUsersViewModel
    {
        public string Id { get; set; }
        public string user { get; set; }
        public string UserName { get; set; }

        public string isFollowed { get; set; }
        public string isBlocked { get; set; }
        public bool isDisabled { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<CodePost> posts { get; set; }
        public DbSet<HashTagPost> hashTags { get; set; }
        public DbSet<HashTag> tags { get; set; }
        public DbSet<UsersVotes> votes { get; set; }
        public DbSet<BlockedUsers> blockedUsers { get; set; }
        public DbSet<UsersFollow> followedUsers { get; set; }
        public DbSet<CommentPost> postComments { get; set; }
        public DbSet<Comment> comments { get; set; }

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

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine(@"",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (DbUpdateException e)
            {
                Console.Write("OVDE TI JE EXCEPTION:   " + e.InnerException);
                //Add your code to inspect the inner exception and/or
                //e.Entries here.
                //Or just use the debugger.
                //Added this catch (after the comments below) to make it more obvious 
                //how this code might help this specific problem
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }

            return 0;
        }
    }
}