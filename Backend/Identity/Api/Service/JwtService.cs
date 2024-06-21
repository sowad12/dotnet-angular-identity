using Api.Models.Entites;
using Api.Models.IOptionModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Service
{
    public class JwtService
    {
        private readonly JwtOption _jwtOption;
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _jwtKey;
        public JwtService(IOptions<JwtOption> jwtOption, IConfiguration configuration)
        {
            _jwtOption = jwtOption.Value;
            _configuration = configuration;
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key));
        }
        public async Task<string> CreateJWT(User user)
        {
          var userClaims=new List<Claim>()
          {
              new Claim(ClaimTypes.NameIdentifier,user.Id),
              new Claim(ClaimTypes.Email,user.Email),
              new Claim(ClaimTypes.GivenName,user.FirstName),
              new Claim(ClaimTypes.Surname,user.LastName)
          };
            var creadentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(_jwtOption.ExpiresInDays),
                SigningCredentials = creadentials,
                Issuer = _jwtOption.Issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwt);
        }
    }
}
