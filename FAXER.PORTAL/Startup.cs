using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FAXER.PORTAL.Startup))]
namespace FAXER.PORTAL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app )
        {
            ConfigureAuth(app);
            ConfigureFireBaseServices();
        }
    }
}
