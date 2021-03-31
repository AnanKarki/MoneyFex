using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Net.Mail;
using System;

namespace FAXER.PORTAL.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
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
        //public ApplicationDbContext()
        //    : base("DefaultConnection", throwIfV1Schema: false)
        //{
        //}

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(null);
        }




        public static ApplicationDbContext Create()
        
        {
            return new ApplicationDbContext();
        }
    }
    public class UserGmailService : SmtpClient
    {


        public string UserName { get; set; }

        public UserGmailService() :
        base(ConfigurationManager.AppSettings["GmailHost"], Int32.Parse(ConfigurationManager.AppSettings["GmailPort"]))
        {
            //Get values from web.config file:

            this.UserName = ConfigurationManager.AppSettings["GmailUserName"];
            this.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["GmailSsl"]);
            this.UseDefaultCredentials = false;
            this.Credentials = new System.Net.NetworkCredential(this.UserName, ConfigurationManager.AppSettings["GmailPassword"]);

        }
    }
}