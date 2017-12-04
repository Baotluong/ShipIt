using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShipIt.Startup))]
namespace ShipIt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
