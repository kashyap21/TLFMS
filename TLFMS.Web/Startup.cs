using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TLFMS.Web.Startup))]

namespace TLFMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}