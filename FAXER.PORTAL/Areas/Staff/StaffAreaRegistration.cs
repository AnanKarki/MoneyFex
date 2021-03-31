using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff
{
    public class StaffAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Staff";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Staff_default",
                "Staff/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional }
                 new { controller = "StaffLogin", action = "StaffMainLogin", id = UrlParameter.Optional }
            );
        }
    }
}