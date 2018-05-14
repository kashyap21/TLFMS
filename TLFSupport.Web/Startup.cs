using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TLFSupport.Web.Startup))]
namespace TLFSupport.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
