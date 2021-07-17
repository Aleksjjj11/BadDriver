using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BadDriver.RestApi
{
    public class AuthOptions
    {
        private static readonly IConfiguration _appConfiguration =
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public static string Issuer => _appConfiguration["AuthOptions:Issuer"];

        public static string Audience => _appConfiguration["AuthOptions:Audience"];

        public static int Lifetime => int.Parse(_appConfiguration["AuthOptions:Lifetime"]);

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["AuthOptions:Key"]));
        }
    }
}