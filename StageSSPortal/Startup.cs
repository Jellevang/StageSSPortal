using Microsoft.Owin;
using Owin;
using StageSSPortal;

[assembly: OwinStartupAttribute(typeof(Startup))]
namespace StageSSPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}