namespace Satellizer.Models
{
    public class ExternalInfo
    {
        public string Code { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
    }

    public class SignUpInfo
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginInfo
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}