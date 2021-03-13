using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Functions.Core.Interfaces
{
    public interface IAuthService
    {
        Task<IAuthResult> ValidateTokenAsync(HttpRequest req, ILogger log);
        Task<IUser> GetUserFromAuth(ClaimsPrincipal principal, ILogger log);
    }
}
