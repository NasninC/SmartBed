using Microsoft.EntityFrameworkCore;
using SmartBed.Data;
using SmartBed.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();


builder.Services.AddSession();
builder.Services.AddSignalR();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<BedHub>("/bedHub");
app.Run();