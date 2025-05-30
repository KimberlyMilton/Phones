using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Phones.Startup))]
namespace Phones
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
