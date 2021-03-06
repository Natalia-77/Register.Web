using AutoMapper;
using Domain;
using Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Register.Web.Constants;
using Register.Web.CustomExceptions;
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
        private readonly ILogger <AccountController> _logger;

        public AccountController(IMapper mapper,
                                UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                RoleManager<AppRole> roleManager,
                                IJWTConfig tokenService,
                                AppDbContext context,
                                IHostEnvironment host,
                                ILogger <AccountController> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _context = context;
            _host = host;
            _logger = logger;
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

                var role = new AppRole
                {
                    Name = Roles.User
                };
                var result1 = _roleManager.CreateAsync(role).Result;

                if (!result.Succeeded)
                    throw new ServerErrorsException("Server error ");

                await _userManager.AddToRoleAsync(user, role.Name);

                await _signInManager.SignInAsync(user, isPersistent: false);

                 DateTime dateTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                _logger.LogInformation($"New user was created{string.Format("{0:d}", dateTime)}");

                return Ok(new
                {
                    token = _tokenService.CreateToken(user)
                });
           
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);        

            if (user == null)
            {                
                throw new ResultEmptyException("User does not exist!");               
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            _logger.LogInformation($"Login user-> {model.Email}");

            if (!result.Succeeded)
            {
                throw new ServerErrorsException("Server error ");
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
            DateTime dateTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);           
            _logger.LogInformation($"Get all user list at {string.Format("{0:d}", dateTime)} ");
            var users = await _context.Users.Select(video => _mapper.Map<UserViewModel>(video)).ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var res = await _context.Users.Where(x => x.Id == id)
                      .Select(userItem => _mapper.Map<UserViewModel>(userItem)).FirstAsync();

            _logger.LogInformation($"Get user {res.Name} info");

            if (res == null)
            {
                throw new ResultEmptyException("User does not exist!");
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
                throw new ResultEmptyException("User model empty");
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
            _logger.LogInformation($"User {res.Email} was updated");

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
                throw new ResultEmptyException("User does not exist!");
            }
            _logger.LogInformation($"User {res.Email} was deleted");
            _context.Users.Remove(res);
            _context.SaveChanges();
            return Ok(new { message = "User deleted" });
        }



    }
}
