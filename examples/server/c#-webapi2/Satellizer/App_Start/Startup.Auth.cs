using System;
using Microsoft.Owin.Cors;
using Owin;
using Satellizer.Models;
using Satellizer.Providers;

namespace Satellizer
{
    public partial class Startup
    {
        public static SatellizerJwtBearerOptions OAuthBearerOptions { get; private set; }

        private static string PublicClientId { get; set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseCors(CorsOptions.AllowAll);

            PublicClientId = "self";

            OAuthBearerOptions = new SatellizerJwtBearerOptions("localhost", PublicClientId,
                "+UX5jSMcWqjNVJED2t4JLjtwCkcqxA7al3M5APPLtNK=", TimeSpan.FromHours(1));

            app.UseJwtBearerAuthentication(OAuthBearerOptions);

            GoogleOAuth2Handler.ClientId = "631036554609-v5hm2amv4pvico3asfi97f54sc51ji4o.apps.googleusercontent.com";
            GoogleOAuth2Handler.ClientSecret = "Google Client Secret";

        }
    }
}
