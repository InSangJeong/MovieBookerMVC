using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieBooker.Startup))]
namespace MovieBooker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
