using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddPooledDbContextFactory<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"))
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddMicrosoftIdentityWebApp(builder.Configuration);

builder.Services
    .RegisterAllServices()
    .AddCors()
    .AddHttpContextAccessor()
    .AddGraphQLServer()
    .RegisterDbContext<AppDbContext>(DbContextKind.Pooled)
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();


app.UseHttpsRedirection();
if(app.Environment.IsDevelopment()) 
    app.UseCors(_ => _.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGraphQL().WithOptions(serverOptions: new GraphQLServerOptions {
    Tool = {
        Enable = app.Environment.IsDevelopment()
    }
});

app.Run();
