using Newtonsoft.Json;

namespace Satellizer.Models.Google
{
    public class GoogleOAuthProfileEmails
    {
        [JsonProperty("value")]
        public string Email { get; set; }
        [JsonProperty("type")]
        public string EmailType { get; set; }
    }
}