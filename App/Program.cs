using App.Authentication;
using AppLogic.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(_ => _.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore);
builder.Services.AddDatabaseServices(builder.Configuration.GetConnectionString("AppDbContext"));
builder.Services.AddCommonServices();
builder.Services.AddCustomerServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddCors();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;   
}).AddMicrosoftIdentityWebApp(builder.Configuration);

builder.Services.Configure<CookieAuthenticationOptions>(o =>
{
    o.LoginPath = PathString.Empty;
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


var app = builder.Build();

app.UseHttpsRedirection();

if(app.Environment.IsDevelopment()) {
    app.UseCors(_ => _.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200").AllowCredentials());
    app.UseHttpLogging();
} else {
    app.UseHsts();
}
    
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
