using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSocket.Web.Startup))]
namespace WebSocket.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
