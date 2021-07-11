using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BadDriver.RestApi
{
    public class AuthOptions
    {
        private const string Key = "taskeMyKey!8279_";   // ключ для шифрации
        public const string Issuer = "TaskeGroupServer"; // издатель токена
        public const string Audience = "BadDriverApp"; // потребитель токена
        public const int Lifetime = 5; // время жизни токена - 1 минута

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}