using Microsoft.EntityFrameworkCore;
using RemoteWork.Data;
using RemoteWork.Models;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using RemoteWork.Hubs;

var builder = WebApplication.CreateBuilder(args);

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? builder.Configuration["DATABASE_URL"];
if (databaseUrl == null)
    throw new InvalidOperationException("DATABASE_URL not found");
var databaseUri = new Uri(databaseUrl);
var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = databaseUri.Host,
    Port = databaseUri.Port,
    Username = databaseUri.UserInfo.Split(':')[0],
    Password = databaseUri.UserInfo.Split(':')[1],
    Database = databaseUri.AbsolutePath.TrimStart('/'),
}.ToString();
Console.WriteLine(connectionString);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSignalR();
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        options.Lockout.AllowedForNewUsers = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Tokens.AuthenticatorTokenProvider = null!;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
app.MapRazorPages();
app.MapHub<ChatHub>("/TeamChat");
app.Run();