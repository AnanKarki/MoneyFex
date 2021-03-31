using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal
{
    public class KiiPayPersonalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KiiPayPersonal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KiiPayPersonal_default",
                "KiiPayPersonal/{controller}/{action}/{id}",
                new { action = "Index", controller = "KiiPayBusinessHome", id = UrlParameter.Optional }
            );
        }
    }
}