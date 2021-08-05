using Newtonsoft.Json;

namespace MobileApps.Models.Responses
{
    public class TokensResponse
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
        [JsonProperty("refreshToken/tokenString")]
        public string RefreshToken { get; set; }
        [JsonProperty("refreshToken/username")]
        public string Username { get; set; }
    }
}