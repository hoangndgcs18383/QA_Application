using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using QA_Application.Data;
using QA_Application.Models;
using QA_Application.Services;

var builder = WebApplication.CreateBuilder(args);

//Add service mailkit
builder.Services.AddOptions();
var mailSettings = builder.Configuration.GetSection("MailSetting");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddTransient<SendMailService>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


//Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(2);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    //Sign In settings
    options.SignIn.RequireConfirmedAccount = false;
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

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

/*app.UseStaticFiles(new StaticFileOptions() { 
    FileProvider = new PhysicalFileProvider(
       Path.Combine(Directory.GetCurrentDirectory(), "Uploads")  
    ),
    RequestPath = "/Uploads"
});*/

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "areas",
        areaName: "Dashboard",
        pattern: "Dashboard/{controller=Home}/{action=Index}/{id?}"
        );
    endpoints.MapAreaControllerRoute(
        name: "areas",
        areaName: "Files",
        pattern: "Dashboard/{controller=Home}/{action=Index}/{id?}"
        );
    endpoints.MapAreaControllerRoute(
        name: "areas",
        areaName: "Manager",
        pattern: "Manager/{controller=Home}/{action=Index}/{id?}"
        );
    endpoints.MapAreaControllerRoute(
        name: "areas",
        areaName: "Database",
        pattern: "Database/{controller=Home}/{action=Index}/{id?}"
        );
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");



    endpoints.MapGet("/TestMail", async (context) =>
    {
        var message = await QA_Application.Services.MailUtils.SendMail("hoangndgcs18383@gmail.com", "hoangndgcs18383@gmail.com", "Test", "Xin chao");

        await context.Response.WriteAsync(message);
    });

    endpoints.MapGet("/TestGmail", async (context) =>
    {
        var message = await QA_Application.Services.MailUtils.SendGmail("hoangndgcs18383@gmail.com", "hoangndgcs18383@gmail.com", "Test", "Xin chao", "hoangndgcs18383@gmail.com", "Jiyeonpark");

        await context.Response.WriteAsync(message);
    });

    endpoints.MapGet("/TestGmailEE", async context => {

        // Lấy dịch vụ sendmailservice
        var sendmailservice = context.RequestServices.GetService<SendMailService>();

        MailContent content = new MailContent
        {
            To = "xuanthulab.net@gmail.com",
            Subject = "Kiểm tra thử",
            Body = "<p><strong>Xin chào xuanthulab.net</strong></p>"
        };

        await sendmailservice.SendMail(content);
        await context.Response.WriteAsync("Send mail");
    });

});



app.MapRazorPages();


app.Run();
