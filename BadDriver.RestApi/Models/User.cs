using System.ComponentModel;

namespace BadDriver.RestApi.Models
{
    public class User
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        [Description("Admin")]
        Admin,
        [Description("User")]
        User
    }
}