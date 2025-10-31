using BusinessLogic.Rules;
using BusinessLogic.Services;
using BusinessLogic.Validation;
using DataAccess.Context;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Presentation {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

            builder.Services.AddScoped<ITagRepository, TagRepository>();
            //builder.Services.AddScoped<TagValidator>();
            //builder.Services.AddScoped<TagRules>();
            builder.Services.AddScoped<TagService>();

            builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
            builder.Services.AddScoped<NewsArticleValidator>();
            builder.Services.AddScoped<NewsArticleRules>();
            builder.Services.AddScoped<NewsArticleService>();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<CategoryValidator>();
            builder.Services.AddScoped<CategoryRules>();
            builder.Services.AddScoped<CategoryService>();

            builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
            builder.Services.AddScoped<SystemAccountValidator>();
            builder.Services.AddScoped<SystemAccountRules>();
            builder.Services.AddScoped<SystemAccountService>();

            builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages();

            app.Run();
        }
    }
}
