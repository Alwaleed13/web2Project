using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using web2Project.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<web2ProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("web2ProjectContext") ?? throw new InvalidOperationException("Connection string 'web2ProjectContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(30);
});

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
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UsersAccount}/{action=Login}/{id?}");

app.Run();
