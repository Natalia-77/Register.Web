using Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Register.Web.Constants;
using Register.Web.Helper;
using Register.Web.Models;
using Register.Web.Services;


namespace Register.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IJWTConfig _tokenService;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                RoleManager<AppRole> roleManager,
                                IJWTConfig tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        //[SupportedOSPlatform("windows")]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
           
                var dir = Directory.GetCurrentDirectory();
                var dirSave = Path.Combine(dir, "images");
                var imageName = Path.GetRandomFileName() + ".jpg";
                var imageSaveFolder = Path.Combine(dirSave, imageName);
                imageSaveFolder.Base64ToImage(model.Photo);           
            
                      
            try
            {
                var role = new AppRole
                {
                    Name = Roles.User
                };
                var result1 = _roleManager.CreateAsync(role).Result;

                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.Name,
                    Photo =imageName

                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return BadRequest(new { message = result.Errors });

                await _userManager.AddToRoleAsync(user, role.Name);

                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok(new
                {
                    token = _tokenService.CreateToken(user)
                });
            }
            catch
            {
                return BadRequest(new { message = "Error database" });
            }

        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = " Something  wrong..." });
            }

            return Ok(new
            {
                token = _tokenService.CreateToken(user)
            });
        }

    }
}
