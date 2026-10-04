//using IDS.Web.UI.Data;
using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
using IDS.DataAccess;
using IDS.Tool;
using IDS.Maintenance;
using IDS.GeneralTable;
using System.Globalization;
using IDS.Web.UI.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllersWithViews()
        .AddRazorRuntimeCompilation()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // Preserve PascalCase
            options.JsonSerializerOptions.MaxDepth = 64;//max json length
        });
}
else
{
    builder.Services.AddControllersWithViews()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // Preserve PascalCase
            options.JsonSerializerOptions.MaxDepth = 64;
        }); ;
}

//Report View Subfolder route -- By Renaldi
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationExpanders.Add(new AreaSubfolderViewLocationExpander());
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(360);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//For Fast Report -- By Renaldi
builder.Services.AddFastReport();

var app = builder.Build();

//For Fast Report -- By Renaldi
FastReport.Utils.RegisteredObjects.AddConnection(typeof(FastReport.Data.MsSqlDataConnection));

HttpContextHelper.Accessor = app.Services.GetRequiredService<IHttpContextAccessor>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.Use(async (context, next) =>
{
    AppPathBase.PathBase = context.Request.PathBase.Value ?? "";
    await next();
});
app.UseAuthorization();

//For Fast Report -- By Renaldi
app.UseFastReport();
var accessor = app.Services.GetRequiredService<IHttpContextAccessor>();
SessionHelper.Configure(accessor);
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
