using System;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Owin.Security.Jwt;
using Newtonsoft.Json.Linq;

namespace Satellizer.Providers
{
    public class SatellizerJwtBearerOptions : JwtBearerAuthenticationOptions
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly TimeSpan _expireTimeSpan;
        private readonly byte[] _key;
        private const string SignatureAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
        private const string DigestAlgorithm = "http://www.w3.org/2001/04/xmlenc#sha256";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="base64Secret">The symmetric key a JWT is signed with</param>
        /// <param name="expireTimeSpan"></param>
        public SatellizerJwtBearerOptions(string issuer, string audience, string base64Secret, TimeSpan expireTimeSpan)
        {
            _issuer = issuer;
            _audience = audience;
            _expireTimeSpan = expireTimeSpan;

            _key = Convert.FromBase64String(base64Secret);

            AllowedAudiences = new[] { audience };
            IssuerSecurityTokenProviders = new[]
            {
                new SymmetricKeyIssuerSecurityTokenProvider(issuer, _key)
            };
        }

        private string Protect(ClaimsIdentity identity)
        {
            var signingCredentials = new SigningCredentials(new InMemorySymmetricSecurityKey(_key), SignatureAlgorithm, DigestAlgorithm);
            var token = new JwtSecurityToken(_issuer, _audience, identity.Claims, DateTime.UtcNow, TokenExpireDateTime.UtcDateTime, signingCredentials); 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private DateTimeOffset TokenExpireDateTime
        {
            get { return DateTimeOffset.UtcNow.Add(_expireTimeSpan); }
        }

        public JObject CreateAuthResponse(ClaimsIdentity identity, string userName)
        {
            return new JObject(
                new JProperty("userName", userName),
                new JProperty("token", Protect(identity)),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", _expireTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture)),
                new JProperty(".issued", DateTimeOffset.UtcNow.ToString()),
                new JProperty(".expires", TokenExpireDateTime.ToString())
                );

        }
    }
}