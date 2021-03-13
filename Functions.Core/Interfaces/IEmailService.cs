using Functions.Core.Models;

namespace Functions.Core.Interfaces
{
    public interface IEmailService
    {
        void SendAsync(Email email);
    }
}
