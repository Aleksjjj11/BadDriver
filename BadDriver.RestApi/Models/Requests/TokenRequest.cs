using System.ComponentModel.DataAnnotations;

namespace BadDriver.RestApi.Models.Requests
{
    public class TokenRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}