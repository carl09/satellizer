using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satellizer.Models.Google;

namespace Satellizer.Models
{
    public class GoogleOAuth2Handler
    {

        public static string ClientId { get; set; }
        public static string ClientSecret { get; set; }

        private const string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";
        private const string UserInfoEndpoint = "https://www.googleapis.com/plus/v1/people/me";

        private readonly HttpClient _httpClient;
        public GoogleOAuth2Handler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GoogleOAuthProfile> ProcessToken(string code, string redirectUri)
        {
            var body = new List<KeyValuePair<string, string>>();
            body.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            body.Add(new KeyValuePair<string, string>("code", code));
            body.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));

            body.Add(new KeyValuePair<string, string>("client_id", ClientId));
            body.Add(new KeyValuePair<string, string>("client_secret", ClientSecret));

            // Request the token
            HttpResponseMessage tokenResponse = await _httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(body));
            tokenResponse.EnsureSuccessStatusCode();
            string text = await tokenResponse.Content.ReadAsStringAsync();

            // Deserializes the token response
            JObject response = JObject.Parse(text);
            string accessToken = response.Value<string>("access_token");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            // Get the Google user info
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage graphResponse = await _httpClient.SendAsync(request);
            graphResponse.EnsureSuccessStatusCode();
            text = await graphResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleOAuthProfile>(text);
        }
    }
}