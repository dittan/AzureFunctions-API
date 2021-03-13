using Functions.Core.Interfaces;
using Functions.Core.Interfaces.Repositorys;
using Functions.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _log;
        private readonly string ISSUER = Environment.GetEnvironmentVariable("AUTH_ISSUER");
        private readonly string AUDIENCE = Environment.GetEnvironmentVariable("AUTH_AUDIENCE");
        private readonly string AUTHORITY = Environment.GetEnvironmentVariable("AUTH_AUTHORITY");
        private readonly string USEREMAIL = Environment.GetEnvironmentVariable("API_USEREMAIL");

        public AuthService(IUserRepository userRepository, ILogger<AuthService> log)
        {
            HttpDocumentRetriever documentRetriever = new HttpDocumentRetriever { RequireHttps = ISSUER.StartsWith("https://", StringComparison.OrdinalIgnoreCase) };

            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{AUTHORITY}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
            _userRepository = userRepository;
            _log = log;
            _log.LogInformation("AuthService instantiated");
        }

        public async Task<IUser> GetUserFromAuth(ClaimsPrincipal principal, ILogger log)
        {
            List<Claim> claims = principal?.Identities.FirstOrDefault().Claims.ToList();
            User user = new User()
            {
                Id = claims.Find(r => r.Type == "sub").Value,
                Email = claims.Find(r => r.Type == USEREMAIL).Value
            };

            return user;
        }

        public async Task<IAuthResult> ValidateTokenAsync(HttpRequest req, ILogger log)
        {
            string jwt = GetTokenFromRequest(req);
            IAuthResult authResult = new AuthResult();
            if (string.IsNullOrEmpty(jwt))
            {
                return authResult;
            }
            authResult.Token = jwt;
            log.LogDebug("start validating token");
            // ConfigurationManager will cache keys for 1d https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/master/src/Microsoft.IdentityModel.Protocols/Configuration/ConfigurationManager.cs
            OpenIdConnectConfiguration config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
            log.LogDebug("got config");
            ClaimsPrincipal principal = null;
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidAudience = AUDIENCE,
                ValidateAudience = true,
                ValidIssuer = ISSUER,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = config.SigningKeys
            };

            int tries = 0;

            while (principal == null && tries <= 1)
            {
                try
                {
                    log.LogDebug("validation starting");
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    // see https://mderriey.com/2019/06/23/where-are-my-jwt-claims/
                    handler.InboundClaimTypeMap.Clear();
                    principal = handler.ValidateToken(jwt, validationParameters, out SecurityToken token);

                    log.LogDebug("validation complete");
                    authResult.Principal = principal;
                    return authResult;
                }
                catch (SecurityTokenSignatureKeyNotFoundException ex1)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a 
                    // refresh and retry validation.
                    _configurationManager.RequestRefresh();
                    tries++;
                    log.LogInformation($"attempt {tries} failed {ex1.Message}, will refresh key");
                }
                catch (SecurityTokenException ex2)
                {
                    log.LogInformation($"token validation failed: {ex2.Message}");
                    authResult.Message = ex2.Message;
                    break;
                }
            }
            return authResult;
        }

        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
        private string GetTokenFromRequest(HttpRequest req)
        {
            if (req.Headers.ContainsKey(AUTH_HEADER_NAME) &&
                req.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                return req.Headers[AUTH_HEADER_NAME].ToString().Substring(BEARER_PREFIX.Length);
            }
            else
            {
                return null;
            }
        }
    }
}
