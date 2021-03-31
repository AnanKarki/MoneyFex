using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers
{
    public class CardUsersAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CardUsers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CardUsers_default",
                "CardUsers/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional }
                new { controller = "CardUserHome", action = "Home", id = UrlParameter.Optional }

            );
        }
    }
}