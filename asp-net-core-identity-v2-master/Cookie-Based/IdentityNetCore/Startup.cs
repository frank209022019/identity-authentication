using IdentityNetCore.Data;
using IdentityNetCore.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IdentityNetCore
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
            //  Configure SQL Server
            var connString = Configuration["ConnectionStrings:LocalHost"];
            services.AddDbContext<ApplicationDBContext>(o => o.UseSqlServer(connString));
            //  Add Identity
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>();
            //   Identity Options
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.SignIn.RequireConfirmedEmail = true;
            });
            //  Application Cookies & Paths
            services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Identity/Signin";
                option.AccessDeniedPath = "/Identity/AccessDenied";
                option.ExpireTimeSpan = TimeSpan.FromHours(10);
            });
            //  SMTP Configuration Options
            services.Configure<SmtpOptions>(Configuration.GetSection("Smtp"));
            //  Services
            services.AddSingleton<IEmailSender, SmtpEmailSender>();
            //  AddControllersWithViews
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}