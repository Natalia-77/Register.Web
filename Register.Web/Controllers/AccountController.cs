using AutoMapper;
using Domain;
using Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;
        private readonly IHostEnvironment _host;


        public AccountController(IMapper mapper,
                                UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                RoleManager<AppRole> roleManager,
                                IJWTConfig tokenService,
                                AppDbContext context,
                                IHostEnvironment host)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _context = context;
            _host = host;
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
                return BadRequest(new { message = "User does not exist!" });
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

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsersList()
        {
            var users = await _context.Users.Select(video => _mapper.Map<UserViewModel>(video)).ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var res = await _context.Users.Where(x => x.Id == id)
                      .Select(userItem => _mapper.Map<UserViewModel>(userItem)).FirstAsync();

            if (res == null)
            {
                return BadRequest(new { message = "Запису з таким id не існує" });
            }

            return Ok(res);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateUser( int id,[FromBody] UpdateUserModel usermodel)
        {
            var res = _context.Users.FirstOrDefault(x => x.Id == id);

            if (usermodel == null)
            {
                return BadRequest(new { message = "No model data" });
            }

            if (!string.IsNullOrEmpty(usermodel.Email))
            {
                res.Email = usermodel.Email;

            }
            if (!string.IsNullOrEmpty(usermodel.Name))
            {
                res.UserName = usermodel.Name;
            }


            if (!string.IsNullOrEmpty(usermodel.Photo))
            {
                var img = ImageConverter.FromBase64StringToImage(usermodel.Photo);
                string randomFilename = Path.GetRandomFileName() + ".jpg";
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "images", randomFilename);
                img.Save(dir, ImageFormat.Jpeg);

                var oldImage = res.Photo;
                string fol = "\\images\\";
                string contentRootPath = _host.ContentRootPath + fol + oldImage;

                if (System.IO.File.Exists(contentRootPath))
                {
                    System.IO.File.Delete(contentRootPath);
                }
                res.Photo = randomFilename;
            }
            _context.SaveChanges();

            return Ok(new { message = "User updated" });

        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// "id":""
        /// </remarks>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var res = _context.Users.FirstOrDefault(x => x.Id == id);
            if (res == null)
            {
                return BadRequest(new { message = "Check id!" });
            }

            _context.Users.Remove(res);
            _context.SaveChanges();
            return Ok(new { message = "User deleted" });
        }



    }
}
