using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

namespace CodeTweets.Models
{
    public class CodePost
    {
        public CodePost()
        {
            date = DateTime.Now;
        }

        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int votes { get; set; }
        public string user_id { get; set; }
        public string userName { get; set; }
        public int like { get; set; }
        public int hate { get; set; }
        public DateTime date { get; set; }

        public virtual IEnumerable<HashTagPost> tagPosts { get; set; }
        public virtual IEnumerable<UsersVotes> usersVotes { get; set; }
        public virtual IEnumerable<CommentPost> postComments { get; set; }
    }

    public class ProfilePostsViewModel
    {

        public ProfilePostsViewModel()
        {
           // commentList = new List<Comment>();
        }

        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string userName { get; set; }
        public int like { get; set; }
        public int hate { get; set; }
        public string liked { get; set; }
        public string hated { get; set; }

        public IEnumerable<Comment> commentList { get; set; }

    }

    /*public class CodePostDbContext: DbContext
    {
        public DbSet<CodePost> posts { get; set; }
        public DbSet<HashTagPost> hashTags { get; set; }
        public DbSet<HashTag> tags { get; set; }
        public DbSet<UsersVotes> votes { get; set; }

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

       
    }*/

}