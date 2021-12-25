using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class AuthOptions
    {
        public const string ISSUER = "KipServer"; // издатель токена
        public const string AUDIENCE = "KipClient"; // потребитель токена
        const string KEY = "mysuperseQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsImV4cCI6MTY0MDM3MTc0NiwiaWF0IjoxNjQwMzcxNzQ2fQ.n5Fy2TivLw23";   // ключ для шифрации
        public const int LIFETIME = 80000; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
