using System;

namespace BadDriver.RestApi.Jwt
{
    public class RefreshToken
    {
        public string Username { get; set; }
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}