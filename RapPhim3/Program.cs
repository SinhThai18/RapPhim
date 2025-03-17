using Microsoft.EntityFrameworkCore;
using RapPhim3.Models;
using RapPhim3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RapPhimContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RapPhimDB")));

builder.Services.AddHttpClient();

builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ShowTimeService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddMemoryCache();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
