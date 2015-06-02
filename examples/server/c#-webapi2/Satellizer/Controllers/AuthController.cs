using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using Satellizer.Models;

namespace Satellizer.Controllers
{
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {

        #region Owin Bits

        private ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        #endregion

        public HttpResponseMessage PostFacebook(string code)
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented, new HttpError("Have not written code yet"));
        }

        [Route("signup")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostSignUp(SignUpInfo signUpInfo)
        {
            var user = await UserManager.FindByEmailAsync(signUpInfo.Email);

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                var appUser = new ApplicationUser
                {
                    UserName = signUpInfo.Email,
                    Email = signUpInfo.Email,
                    DisplayName = signUpInfo.DisplayName,
                };

                var result = UserManager.Create(appUser, signUpInfo.Password);

                if (result.Succeeded)
                {
                    var accessTokenResponse = GenerateLocalAccessTokenResponse(appUser);

                    return Request.CreateResponse(HttpStatusCode.OK, accessTokenResponse);
                }
                
                return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError(result.Errors.First()));
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);

        }

        [Route("login")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostLogin(LoginInfo loginInfo)
        {
            var user = await UserManager.FindAsync(loginInfo.Email, loginInfo.Password);

            if (user != null)
            {
                var accessTokenResponse = GenerateLocalAccessTokenResponse(user);

                return Request.CreateResponse(HttpStatusCode.OK, accessTokenResponse);
            }
            return Request.CreateResponse((HttpStatusCode)422, new HttpError("Bad username or password"));
        }

        [Route("unlink/{loginProvider}")]
        [Authorize]
        public async Task<HttpResponseMessage> GetUnlink(string loginProvider)
        {
            var appUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var userLoginInfo = appUser.Logins.FirstOrDefault(x => x.LoginProvider.Equals(loginProvider, StringComparison.InvariantCultureIgnoreCase));
            if (userLoginInfo != null)
            {
                var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey));

                return result.Succeeded ? 
                    Request.CreateResponse(HttpStatusCode.NoContent) : 
                    Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError(result.Errors.FirstOrDefault()));
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, new HttpError("Login providor not found"));
        }

        [Route("Google")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostGoogle(ExternalInfo externalInfo)
        {
            GoogleOAuth2Handler auth2AuthenticationHandler = new GoogleOAuth2Handler(new HttpClient());

            var googleOAuthProfile = await auth2AuthenticationHandler.ProcessToken(externalInfo.Code, externalInfo.RedirectUri);

            var clameIdentity = googleOAuthProfile.Id;
            var email = googleOAuthProfile.Emails.First().Email;

            ApplicationUser appUser = await UserManager.FindAsync(new UserLoginInfo("Google", clameIdentity));

            bool hasRegisteredWithAccount = appUser != null;

            if (!hasRegisteredWithAccount)
            {
                var info = new ExternalLoginInfo
                {
                    DefaultUserName = email,
                    Login = new UserLoginInfo("Google", clameIdentity)
                };

                try
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        appUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        if (string.IsNullOrEmpty(appUser.Picture))
                        {
                            appUser.Picture = googleOAuthProfile.Image.Url;
                            await UserManager.UpdateAsync(appUser);
                        }
                    }
                    else
                    {
                        appUser = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            DisplayName = googleOAuthProfile.DisplayName,
                            Picture = googleOAuthProfile.Image.Url
                        };
                        UserManager.Create(appUser);
                    }

                    UserManager.AddLogin(appUser.Id, info.Login);
                }
                catch (Exception exception)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new HttpError(exception, true));
                }
            }
            
            var accessTokenResponse = GenerateLocalAccessTokenResponse(appUser);

            return Request.CreateResponse(HttpStatusCode.OK, accessTokenResponse);
        }

        public HttpResponseMessage PostLinkedin(string code)
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented, new HttpError("Have not written code yet"));
        }

        public HttpResponseMessage GetTwitter(string oauth)
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented, new HttpError("Have not written code yet"));
        }

        private static JObject GenerateLocalAccessTokenResponse(ApplicationUser applicationUser)
        {
            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, applicationUser.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, applicationUser.Id));
            identity.AddClaim(new Claim("sub", applicationUser.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

            return Startup.OAuthBearerOptions.CreateAuthResponse(identity, applicationUser.UserName); 
        }

    }
}