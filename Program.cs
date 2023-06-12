using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FORTTHEGAMES.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using FORTTHEGAMES.Data.Cart;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FORTTHEGAMESContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FORTTHEGAMESContext") ?? throw new InvalidOperationException("Connection string 'FORTTHEGAMESContext' not found.")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/Acceso/Login";
});


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc));
builder.Services.AddSession();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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

//CAR
app.UseSession();
//HDIAZ AUTH
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
