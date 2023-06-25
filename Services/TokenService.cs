using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAPI.Interfaces;

namespace UserAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
        }

        public string GenerateAccessToken()
        {
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
               expires: DateTime.Now.AddHours(int.Parse(_config["AppSettings:TokenExpiryHours"])),
               signingCredentials: creds
               )
            {

            };
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;

        }
    }
}
