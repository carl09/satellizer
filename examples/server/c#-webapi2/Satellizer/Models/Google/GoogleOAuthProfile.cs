using System.Collections.Generic;
using Newtonsoft.Json;

namespace Satellizer.Models.Google
{
    public class GoogleOAuthProfile
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("emails")]
        public List<GoogleOAuthProfileEmails> Emails { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public GoogleOAuthProfileName Name { get; set; }
        [JsonProperty("image")]
        public GoogleOAuthProfileImage Image { get; set; }
    }
}