using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SanitaryWareECommerce.Infrastructure.Persistence;
using SanitaryWareECommerce.Infrastructure;
using MudBlazor.Services;
using SanitaryWareECommerce.Application;

namespace SanitaryWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {

            services.AddRazorPages();
            services.AddServerSideBlazor();


            services.AddInfrastructureServices();
            services.AddApplicationServices();

            // MudBlazor
            services.AddMudServices();

            // Lấy chuỗi kết nối từ appsettings.json
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Đăng ký DbContext với SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
        }

  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
