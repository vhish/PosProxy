using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Authentication
{
    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory baselogger,
            UrlEncoder encoder, ISystemClock clock, IConfiguration configuration, ILogger<CustomAuthHandler> logger,
            IMemoryCache memoryCache)
            : base(options, baselogger, encoder, clock)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get Authorization header value
            var schemeToken = RequestHeaderHelper.GetSchemeAndToken(Request);
            if (string.IsNullOrEmpty(schemeToken.Key) || string.IsNullOrEmpty(schemeToken.Value))
            {
                _logger.LogDebug($"Failed - Schema token Key: {schemeToken.Key}");
                _logger.LogDebug($"Failed - Schema token Value: {schemeToken.Value}");
                return await Task.FromResult(AuthenticateResult.Fail("Authorization header is unrecognized"));
            }

            var scheme = schemeToken.Key.ToLower();
            var token = schemeToken.Value;

            if (scheme.Equals("hubtel-basic", StringComparison.OrdinalIgnoreCase))
            {
                if (ValidateClientIp(Request) && ValidateApiToken(token, out HubtelProfile hubtelProfile))
                {
                    return await HubtelBearerAuthenticateAsync(scheme, hubtelProfile);
                }
            }

            if (scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase))
            {
                if (ValidateApiJwtToken(token, out HubtelProfile hubtelProfile))
                {
                    return await HubtelBearerAuthenticateAsync(scheme, hubtelProfile);
                }
            }

            return await Task.FromResult(AuthenticateResult.Fail("Authorization Failed"));
        }

        private async Task<AuthenticateResult> HubtelBearerAuthenticateAsync(string scheme, HubtelProfile hubtelProfile)
        {
            // Create authenticated user
            var claims = new List<Claim>
            {
                new Claim("Name", hubtelProfile.MobileNumber),
                new Claim("AccountId", hubtelProfile.AccountId),
                new Claim("Phone", hubtelProfile.MobileNumber)
            };

            var identities = new List<ClaimsIdentity>
            {
                new ClaimsIdentity(claims, scheme)
            };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), scheme);

            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private bool ValidateClientIp(HttpRequest request)
        {
            //var salesApiIp = _configuration["AppSettings:SalesApiMicroService:Ip"];
            //var remoteIpAddress = request.HttpContext.Connection.RemoteIpAddress;
            //Console.WriteLine("Remote IP Address:"+remoteIpAddress);
            //Console.WriteLine("Configuration IP Address:"+salesApiIp);
            //return salesApiIp.Contains(remoteIpAddress.ToString());
            return true;
        }
        
        private bool ValidateApiToken(string token, out HubtelProfile hubtelProfile)
        {
            var authKey = _configuration["HubtelAuth:Key"];

            var headerStr = RequestHeaderHelper.Base64Decode(token);
            string[] stringSeparators = new string[] { ":" };
            var headerStrArr = headerStr.Split(stringSeparators, 2, StringSplitOptions.None);
            var basicKeys = $"{headerStrArr[0]}";

            if ($"{authKey}".Equals(basicKeys))
            {
                hubtelProfile = JsonConvert.DeserializeObject<HubtelProfile>(headerStrArr[1]);
                return true;
            }
            hubtelProfile = null;
            return false;
        }

        private bool ValidateApiJwtToken(string token, out HubtelProfile hubtelProfile)
        {
            hubtelProfile = null;

            var issuer = _configuration["AppSettings:HubtelAuth:Issuer"];
            var audience = _configuration["AppSettings:HubtelAuth:Audience"];
            var key = _configuration["AppSettings:HubtelAuth:Key"];

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                }, out SecurityToken validatedToken);

                ClaimsPrincipal principal = claimsPrincipal;
                hubtelProfile = new HubtelProfile
                {
                    AccountId = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sid)?.Value ?? string.Empty,
                    MobileNumber = claimsPrincipal.FindFirst(ClaimTypes.MobilePhone)?.Value ?? string.Empty
                };
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return false;
        }
    }
}
