using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Satellizer.Controllers
{
    [Authorize]
    [RoutePrefix("api/me")]
    public class MeController : ApiController
    {
        private ApplicationUserManager _userManager;

        public MeController()
        {
        }

        public MeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET api/Me
        public HttpResponseMessage Get()
        {
            var result = new GetViewModel();

            var user = UserManager.FindById(User.Identity.GetUserId());

            result.DisplayName = user.DisplayName;
            result.Picture = user.Picture;
            result.Id = user.Id;
            result.Email = user.Email;

            var logins = user.Logins;
            foreach (var login in logins)
            {
                if (login.LoginProvider == "Google")
                {
                    result.Google = login.ProviderKey;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        public async Task<HttpResponseMessage> Put(GetViewModel viewModel)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            user.DisplayName = viewModel.DisplayName;
            user.Email = viewModel.Email;

            await UserManager.UpdateAsync(user);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }

    public class GetViewModel
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string Google { get; set; }
        public string Id { get; set; }
    }
}