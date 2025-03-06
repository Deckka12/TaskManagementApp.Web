using TaskManagementApp.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using TaskManagementApp.Domain.Interface;
using TaskManagementApp.Infrastructure.Repositories;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Application.Services;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace TaskManagementApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Users/Login"; // Страница входа
                    options.LogoutPath = "/Users/Logout"; // Страница выхода
                });

            builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddHttpContextAccessor();
            //builder.WebHost.UseUrls("http://localhost:5001");

            var app = builder.Build();
            app.UseAuthentication();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();


            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.UseDeveloperExceptionPage();
            app.Run();
        }
    }
}
