using API.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPooledDbContextFactory<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"))
);
builder.Services.AddCors();
builder.Services
    .RegisterAllServices()
    .AddHttpContextAccessor()
    .AddGraphQLServer()
    .RegisterDbContext<AppDbContext>(DbContextKind.Pooled)
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();


app.MapGraphQL().WithOptions(serverOptions: new GraphQLServerOptions {
    Tool = {
        Enable = app.Environment.IsDevelopment()
    }
});

if(app.Environment.IsDevelopment()) 
    app.UseCors(_ => _.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());


app.Run();