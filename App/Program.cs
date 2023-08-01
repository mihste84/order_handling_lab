using App.Authentication;
using Common.Interfaces;
using MasterData;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(_ => _.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore);
builder.Services.AddDatabaseServices(builder.Configuration.GetConnectionString("AppDbContext"));
builder.Services.AddCommonServices();
builder.Services.AddCustomerServices();
builder.Services.AddMasterDataServices();
builder.Services.AddHttpContextAccessor();

if (builder.Environment.IsEnvironment("IntegrationTests"))
{
    builder.Services.AddScoped<IAuthenticationService, TestAuthenticationService>();
    builder.Services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
}
else
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddMicrosoftIdentityWebApp(builder.Configuration);

    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    builder.Services.AddCors();
}

builder.Services.Configure<CookieAuthenticationOptions>(o => o.LoginPath = PathString.Empty);

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors(_ => _.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200").AllowCredentials());
    app.UseHttpLogging();
}
else
{
    app.UseHsts();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }