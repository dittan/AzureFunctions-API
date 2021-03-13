using System.Security.Claims;

namespace Functions.Core.Interfaces
{
    public interface IAuthResult
    {
        ClaimsPrincipal Principal { get; set; }
        string Token { get; set; }
        string Message { get; set; }
    }
}
