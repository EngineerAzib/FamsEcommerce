using FamsEcommerce.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System.Web.Services.Description;


[assembly: OwinStartupAttribute(typeof(FamsEcommerce.Startup))]
namespace FamsEcommerce
{
    public partial class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add your services using the AddXXX methods
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

            // ... other service registrations
        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
