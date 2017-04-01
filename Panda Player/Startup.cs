using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Panda_Player.Startup))]
namespace Panda_Player
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
