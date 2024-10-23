using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CCFD.Startup))]
namespace CCFD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
