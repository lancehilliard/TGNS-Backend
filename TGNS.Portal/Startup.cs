using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TGNS.Portal.Startup))]
namespace TGNS.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
