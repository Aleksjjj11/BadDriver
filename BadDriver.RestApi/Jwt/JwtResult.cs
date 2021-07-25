namespace BadDriver.RestApi.Jwt
{
    public class JwtResult
    {
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}