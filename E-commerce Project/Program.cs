using E_commerce_Project.Authentication.CookieEvents;
using E_commerce_Project.Models.Context;
using E_commerce_Project.Models.Entities;
using E_commerce_Project.Models.Services.AuthService;
using E_commerce_Project.Models.Services.CartService;
using E_commerce_Project.Models.Services.CategoryService;
using E_commerce_Project.Models.Services.FileService;
using E_commerce_Project.Models.Services.OrderServive;
using E_commerce_Project.Models.Services.ProductImageService;
using E_commerce_Project.Models.Services.ProductService;
using E_commerce_Project.Models.Services.SliderService;
using E_commerce_Project.Services.UserService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add authentication service
builder.Services.AddAuthentication("CookieAuthCustomer")
    .AddCookie("CookieAuthCustomer", options =>
    {
        options.Cookie.Name = "UserAuthCookie";
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    })
    .AddCookie("CookieAuthAdmin", options =>
    {
        options.Cookie.Name = "AdminAuthCookie";
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(5248);
// });


// Add service handle logic business
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthSerivce>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Service custom cookie event validation
builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "productSlug",
    pattern: "Product/{slug}",
    defaults: new { Controller = "Product", action = "Index" }
);


app.Run();