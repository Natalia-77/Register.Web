using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Register.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Register.Web.Services.Implements
{
    public class JWTConfig : IJWTConfig
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<AppUser> _userManager;
        public JWTConfig(IOptions<AppSettings> appsettings, UserManager<AppUser> userManager)
        {
            _appSettings = appsettings.Value;
            _userManager = userManager;
        }
        public string CreateToken(AppUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            var roleClaims = new List<Claim>()
            {
                 new Claim("id",user.Id.ToString()),
                 new Claim("name",user.UserName)
            };
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var key = Encoding.ASCII.GetBytes(_appSettings.Key);
            var signKey = new SymmetricSecurityKey(key);
            var singCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
               signingCredentials: singCredentials,
               expires: DateTime.Now.AddDays(1),
               claims: roleClaims
               );
            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }


    }

}
