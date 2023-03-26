using Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDatabaseServices(builder.Configuration.GetConnectionString("AppDbContext"));
builder.Services.AddCommonServices();
builder.Services.AddCustomerServices();
builder.Services.AddAuthenticationServices();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddMicrosoftIdentityWebApp(builder.Configuration);

builder.Services
    .RegisterAllServices()
    .AddCors();

var app = builder.Build();

app.UseHttpsRedirection();

if(app.Environment.IsDevelopment()) {
    app.UseCors(_ => _.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());
    app.UseHttpLogging();
} else {
    app.UseHsts();
}
    
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
