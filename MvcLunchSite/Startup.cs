using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcLunchSite.Startup))]
namespace MvcLunchSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
