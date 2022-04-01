using Domain.Identity;

namespace Register.Web.Services
{
    public interface IJWTConfig
    {
        public string CreateToken(AppUser user);
    }
}
