using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CodeTweets.Startup))]
namespace CodeTweets
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
