using System.ComponentModel.DataAnnotations;

namespace BadDriver.RestApi.Models.Requests
{
    public class RefreshAccessRequest
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}