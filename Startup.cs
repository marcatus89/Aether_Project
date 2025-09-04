using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SanitaryWareECommerce.Application;
using SanitaryWareECommerce.Domain.Entities;
using SanitaryWareECommerce.Infrastructure;
using SanitaryWareECommerce.Infrastructure.Persistence;
using MudBlazor.Services;
using System;
using Microsoft.Extensions.Logging;
using SanitaryWareECommerce.Infrastructure.Persistence.Seeders;




namespace SanitaryWeb;

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
        // 1. Cấu hình DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

        // 2. Cấu hình Authentication và Identity
        services.AddAuthentication("Identity.Application").AddCookie();
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // 3. Đăng ký các service của bạn từ các project khác
        services.AddInfrastructureServices();
        services.AddApplicationServices();

        // 4. Các dịch vụ mặc định của Blazor và MudBlazor
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddMudServices();
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
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Đảm bảo thứ tự chính xác: Authentication -> Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        SeedDatabase(app); 

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Thêm dòng này nếu bạn có API Controller
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
    private static void SeedDatabase(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Áp dụng migration (nếu có)
                context.Database.Migrate();

                // Gọi seeder với các tham số mới
                DataSeeder.SeedAsync(context, userManager, roleManager).Wait();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Startup>>();
                logger.LogError(ex, "An error occurred during migration or seeding the database.");
            }
        }
    }

}

