using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Odp.Data;
using Odp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Odp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
            );

            /* @TODO Remeber to test again later: https://github.com/aspnet/AspNetCore.Docs/issues/14944
            services.AddDefaultIdentity<ApplicationUser>()
                            .AddRoles<IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();*/

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    {
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 6;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireLowercase = true;
                        options.Password.RequiredUniqueChars = 0;
                        options.User.RequireUniqueEmail = true;
                        //                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-._ @+";
                        options.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromMinutes(5);
                        options.Lockout.MaxFailedAccessAttempts = 5;
                        options.Lockout.AllowedForNewUsers = true;
                        options.SignIn.RequireConfirmedEmail = true;
                        options.SignIn.RequireConfirmedPhoneNumber = false;
                        options.Stores.ProtectPersonalData = false; //if true, require IProtectedUserStore implementation
//                                    options.Tokens.AuthenticatorIssuer = Configuration["Jwt:Issuer"];
                        //                    options.Tokens.AuthenticatorTokenProvider = ;
                        //                    options.Tokens.ChangeEmailTokenProvider = ;
                        //                    options.Tokens.ChangePhoneNumberTokenProvider = ;
                        //                    options.Tokens.EmailConfirmationTokenProvider = ;
                        //                    options.Tokens.PasswordResetTokenProvider = ;
                    })
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireLoggedIn", policy => policy.RequireRole("Admin","Customer",  "Author", "Moderator").RequireAuthenticatedUser());
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin").RequireAuthenticatedUser());
                options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer").RequireAuthenticatedUser());
                options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Moderator").RequireAuthenticatedUser());
                options.AddPolicy("RequireAuthorRole", policy => policy.RequireRole("Author").RequireAuthenticatedUser());
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            InitIdentityDatabaseAsync(serviceProvider).Wait();
        }
        private async System.Threading.Tasks.Task InitIdentityDatabaseAsync(IServiceProvider serviceProvider)
        {
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            IdentityResult iResult;

            //Adding Roles if not exists
            var roleCheck = await RoleManager.RoleExistsAsync("Admin")
                            && await RoleManager.RoleExistsAsync("User")
                            && await RoleManager.RoleExistsAsync("Author")
                            && await RoleManager.RoleExistsAsync("Moderator");
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                iResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
                iResult = await RoleManager.CreateAsync(new IdentityRole("User"));
                iResult = await RoleManager.CreateAsync(new IdentityRole("Author"));
                iResult = await RoleManager.CreateAsync(new IdentityRole("Moderator"));
                //Assign Admin role to the main User here we have given our newly registered 
                //login id for Admin management
                ApplicationUser defaultUser = await UserManager.FindByEmailAsync("wwwbrtswww@gmail.com");
                if (defaultUser == null)
                {
                    defaultUser = new ApplicationUser
                    {
                        PreferedStyle = ApplicationUser.DEFAULTPREFEREDSTYLE,
                        Society = "Sebastien Deschamps",
                        Gender = "Mr",
                        Firstname = "Sebastien",
                        Lastname = "Deschamps",
                        Fulladdress = "1128 route de la Colombi�re, 74950 Le Reposoir FRANCE",
                        Street = "1128 route de la Colombi�re",
                        Zipcode = 74950,
                        City = "Le Reposoir",
                        Country = "FRANCE",
                        ConnectionTimestamp = (UInt32) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        Email = "wwwbrtswww@gmail.com",
                        UserName = "wwwbrtswww@gmail.com",
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var pass = new String("CH@NG3me");

                    iResult = await UserManager.CreateAsync(defaultUser, pass);
                    if (iResult.Succeeded)
                    {
                        defaultUser = await UserManager.FindByEmailAsync("wwwbrtswww@gmail.com");
                        iResult = await UserManager.AddToRoleAsync(defaultUser, "Admin");
                    }
                    else
                    {
                        throw new Exception("Startup error on populate database Users and Roles in startup.cs");
                    }
                }
            }
        }
    }
}
