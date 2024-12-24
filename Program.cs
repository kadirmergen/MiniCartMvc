using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MiniCartMvc.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniCartMvc.Identity;
using Microsoft.AspNetCore.Identity;


namespace MiniCartMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDistributedMemoryCache(); // In-memory cache
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24); // Session timeout
                options.Cookie.HttpOnly = true; // Cookie güvenliði
                options.Cookie.IsEssential = true; // GDPR uyumu için
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<IdentityDataContext>().AddDefaultTokenProviders();
            
            builder.Services.AddAuthentication("ApplicationCookie").AddCookie("ApplicationCookie", options =>
            {
                options.LoginPath = "/Accounts/Login";
            });
            
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                DbInitializer.Seed(services).Wait();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "theme")),
                RequestPath = "/theme"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}