using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MaKore.Controllers
{
    public class BaseController : Controller
    {
        // Validate the Authorization header
        private bool isValid(string token, IConfiguration _configuration)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWTParams:SecretKey"]));
            var myIssuer = _configuration["JWTParams:Issuer"];
            var myAudience = _configuration["JWTParams:Audience"];
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public  string UserNameFromJWT(string token, IConfiguration _configuration)
        {
            if (isValid(token, _configuration)) {
                var handler = new JwtSecurityTokenHandler();
                //string authHeader = Request.Headers["Authorization"];
                //authHeader = authHeader.Replace("Bearer ", "");
                var jsonToken = handler.ReadToken(token);
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var user = tokenS.Claims.First(claim => claim.Type == "NameIdentifier").Value;
                if (user == null)
                    return null;
                else
                    return user;
            } else
            {
                return null;
            }


        }
    }
}
