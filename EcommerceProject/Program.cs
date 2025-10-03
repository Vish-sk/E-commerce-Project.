using EcommerceProject.DBcontext;
using EcommerceProject.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Add Database
builder.Services.AddDbContext<Database>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MVcConnectionString"));
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Cookie Authentication for Sellers
builder.Services.AddAuthentication("SellerCookie")
    .AddCookie("SellerCookie", options =>
    {
        options.LoginPath = "/Home/SellerLogin";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// Add Services
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=ProductView}/{id?}");

app.Run();
