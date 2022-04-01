using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Register.Web.Constants;

namespace Register.Web.Seeder
{
    public static class AdminSeedData
    {
        public static async Task SeedData(this IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

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
