using Domain;
using Domain.Identity;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Register.Web.Mapper;
using Register.Web.Models;
using Register.Web.Seeder;
using Register.Web.Services;
using Register.Web.Services.Implements;
using Register.Web.Validation;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>((DbContextOptionsBuilder options) =>

               options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

//how use interfaces
builder.Services.AddScoped<IJWTConfig, JWTConfig>();

// For Identity
builder.Services.AddIdentity<AppUser, AppRole>(option =>
{
    option.Password.RequireDigit = true;
    option.Password.RequiredLength = 5;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
    option.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Configuration from AppSettings
var appSettingSection = configuration.GetSection("AppSetting");
builder.Services.Configure<AppSettings>(appSettingSection);

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = false;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSetting:Key"]))
    };
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddFluentValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IValidator<RegisterViewModel>, UserValidator>();
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
builder.Services.AddSwaggerGen((SwaggerGenOptions o) =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Description = "Swagger",
        Version = "v1",
        Title = "Video player example"
    });
});
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI((SwaggerUIOptions c) =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Player");
    });
}

app.UseStaticFiles();

string folderName = "images";

var dir = Path.Combine(Directory.GetCurrentDirectory(), folderName);

if (!Directory.Exists(dir))
{
    Directory.CreateDirectory(dir);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dir),
    RequestPath = "/images"
});

app.UseHttpsRedirection();
app.UseRouting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
await app.SeedData();
app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
});

app.Run();