using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LunchSite.Startup))]
namespace LunchSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
