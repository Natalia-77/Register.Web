using AutoMapper;
using Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Register.Web.Constants;
using Register.Web.Helper;
using Register.Web.Models;
using Register.Web.Services;
using System.Drawing.Imaging;

namespace Register.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IJWTConfig _tokenService;

        public AccountController(IMapper mapper,
            UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                RoleManager<AppRole> roleManager,
                                IJWTConfig tokenService)
        {
            _mapper = mapper;
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
            var img = ImageConverter.FromBase64StringToImage(model.Photo);
            string randomFilename = Path.GetRandomFileName() + ".jpg";
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "images", randomFilename);
            img.Save(dir, ImageFormat.Jpeg);
            var user = _mapper.Map<AppUser>(model);
            user.Photo = randomFilename;
            var result = await _userManager.CreateAsync(user, model.Password);


            try
            {
                var role = new AppRole
                {
                    Name = Roles.User
                };
                var result1 = _roleManager.CreateAsync(role).Result;


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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User does not exist!"});
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Something  wrong..." });
            }

            return Ok(new
            {
                token = _tokenService.CreateToken(user)
            });
        }

    }
}
