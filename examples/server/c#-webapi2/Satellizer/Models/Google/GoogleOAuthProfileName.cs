using Newtonsoft.Json;

namespace Satellizer.Models.Google
{
    public class GoogleOAuthProfileName
    {
        [JsonProperty("familyName")]
        public string FamilyName { get; set; }
        [JsonProperty("givenName")]
        public string GivenName { get; set; }
        [JsonProperty("middleName")]
        public string MiddleName { get; set; }
    }
}