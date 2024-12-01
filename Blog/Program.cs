using Blog.DBContext;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=mydatabase.db"));
    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();



    services.AddControllersWithViews();
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
    }).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

    builder.Services.AddControllersWithViews();

    builder.Services.AddControllersWithViews();

    var app = builder.Build();



}
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
