using Newtonsoft.Json;

namespace Satellizer.Models.Google
{
    public class GoogleOAuthProfileImage
    {
        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}