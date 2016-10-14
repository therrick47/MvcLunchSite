using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SuperLunchDecider.Startup))]
namespace SuperLunchDecider
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
