using System.Net;
using home_control;
using home_control.BackgroundServices;
using home_control.Extensions;
using home_control.Hub;
using home_control.Models;
using home_control.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;



var builder = WebApplication.CreateBuilder(args);
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddHostedService(serviceProvider => new ConsumerService("kafkaClient.properties", serviceProvider.GetService<IHubContext<QueueMessageHub, IQueueMessageHub>>()));
#pragma warning restore CS8604 // Possible null reference argument.
builder.Services.AddTransient(serviceProvider => new QueueService("kafkaClient.properties"));
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureSqlContext(builder.Configuration);


//builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
//    {
//    })
//    .AddEntityFrameworkStores<MyDbContext>()
//    .AddDefaultTokenProviders()
//    .AddSignInManager();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = "External";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        //options.LoginPath = "/api/user/loginexternal"; // Must be lowercase
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Events = new CookieAuthenticationEvents()
        {
            OnRedirectToLogin = (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddCookie("External")
    .AddGoogle(options =>
        {
            options.ClientId = "706648688117-i11ou8bmhaj8qlep7li0t5fvptt8pm68.apps.googleusercontent.com";
            options.ClientSecret = "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";
            options.Scope.Add("profile");
        });

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:5173").AllowCredentials();
    options.AllowAnyHeader();
    options.AllowAnyMethod();
});
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<QueueMessageHub>("/hub");

app.MapFallbackToFile("index.html");
app.Run();
