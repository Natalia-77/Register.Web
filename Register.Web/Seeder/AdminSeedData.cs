using Domain;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Register.Web.Constants;

namespace Register.Web.Seeder
{
    public static class AdminSeedData
    {
        //public static async Task SeedData(this IApplicationBuilder applicationBuilder)
        //{
        //    using var serviceScope = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        //    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        //    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        //    if (!userManager.Users.Any())
        //    {
        //        var role = new AppRole
        //        {
        //            Name = Roles.Admin
        //        };
        //        var result1 = roleManager.CreateAsync(role).Result;

        //        var user = new AppUser
        //        {
        //            Email = "ant@gmail.com",
        //            UserName = "ant@gmail.com"
        //        };
        //        var res = await userManager.CreateAsync(user, "zxcvbn22");
        //        res = await userManager.AddToRoleAsync(user, Roles.Admin);
        //    }

        //}

        public static async Task SeedData(this WebApplication webApplication)
        {
            using(var scope = webApplication.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Migration is on progress\n");
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                    await SeedDataDb(services);
                }
                catch(Exception ex)
                {
                    logger.LogError("Error seed database -> " + ex.Message);
                }
            }

        }

        private static async Task SeedDataDb(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            if (!roleManager.Roles.Any())
            {
                var result = roleManager.CreateAsync(new AppRole
                {
                    Name = Roles.Admin
                }).Result;

                result = roleManager.CreateAsync(new AppRole
                {
                    Name = Roles.User
                }).Result;
            }

            if (!userManager.Users.Any())
            {
                var role = new AppRole
                {
                    Name = Roles.Admin
                };
                var result1 = roleManager.CreateAsync(role).Result;

                var user = new AppUser
                {
                    Email = "ant@gmail.com",
                    UserName = "ant@gmail.com"
                };
                var res = await userManager.CreateAsync(user, "zxcvbn22");
                res = await userManager.AddToRoleAsync(user, Roles.Admin);
            }
        }
    }
}
