using Functions.Core.Interfaces;

namespace Functions.Core.Models
{
    public class User : IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
