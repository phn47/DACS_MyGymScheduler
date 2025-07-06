using toanggg.Data;
using toanggg.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using toanggg.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using toanggg.Controllers;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Configuration;
/*using toanggg.Hubs;*/



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LinhContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("QLPHONGTAP"));
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<EmailSender>(builder.Configuration.GetSection("EmailSettings"));

// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
/*builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/DangNhap";
        options.AccessDeniedPath = "/Home/Privacy";
    });*/
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Users/DangNhap";
    options.AccessDeniedPath = "/Home/Privacy";
})

.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    options.ClaimActions.MapJsonKey("urn:facebook:picture", "picture.data.url");
    options.SaveTokens = true;
})

   .AddGoogle(googleOptions =>
   {
       googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
       googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
       googleOptions.CallbackPath = "/signin-google";
   });

builder.Services.AddScoped<SearchService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IVnPayService, VnPayService>();

// Add Hangfire services.
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});

builder.Services.AddScoped<EmailService>();
builder.Services.AddHostedService<WeeklyEmailService>();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Hangfire server.
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Use Hangfire dashboard.
app.UseHangfireDashboard();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "displaySchedulesWithNullRoom",
        pattern: "ClassSchedules/DisplaySchedulesWithNullRoom",
        defaults: new { controller = "ClassSchedules", action = "DisplaySchedulesWithNullRoom" });
    endpoints.MapHangfireDashboard();
    /*   endpoints.MapHub<ChatHub>("/chatHub");*/
});

RecurringJob.AddOrUpdate<EmailService>("send-weekly-emails", x => x.SendWeeklyEmails(), Cron.Weekly(DayOfWeek.Monday, hour: 0, minute: 0));

app.Run();
