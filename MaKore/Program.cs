using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MaKoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MaKoreContext") ?? throw new InvalidOperationException("Connection string 'MaKoreContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromSeconds(10));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Register}/{id?}");


app.Run();
