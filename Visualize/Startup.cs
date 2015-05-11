using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Visualize.Startup))]
namespace Visualize
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
