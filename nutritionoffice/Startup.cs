using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(nutritionoffice.Startup))]
namespace nutritionoffice
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
