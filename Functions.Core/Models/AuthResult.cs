using Functions.Core.Interfaces;
using System.Security.Claims;

namespace Functions.Core.Models
{
    public class AuthResult : IAuthResult
    {
        public ClaimsPrincipal Principal { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
