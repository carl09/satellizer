using Microsoft.Owin;
using Owin;
using Satellizer;

[assembly: OwinStartup(typeof(Startup))]

namespace Satellizer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
