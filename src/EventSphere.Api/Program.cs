using System.Reflection;
using System.Text;
using DotNetEnv;
using EventSphere.Api.Configuration;
using EventSphere.Api.Middlewares;
using EventSphere.Api.Swagger.Filters;
using EventSphere.Application.Services;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Infrastructure.Data;
using EventSphere.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.GridFS;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

Env.Load();
config.AddEnvironmentVariables();

// jwt authentication
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = config["JwtSettings:Issuer"],
            ValidAudience = config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT_SECRET_KEY"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddCookie()
    .AddGoogle("GoogleLogin", options =>
    {
        options.ClientId = config["GOOGLE_CLIENT_ID"]!;
        options.ClientSecret = config["GOOGLE_CLIENT_SECRET"]!;
        options.SaveTokens = false;
        options.Scope.Add("email");
    });


builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
{
    var connString = config["ConnectionStrings:PostgresDb"];
    optionsBuilder.UseNpgsql(connString);
});

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<IGridFSBucket>(
    provider => provider.GetRequiredService<MongoDbContext>().GetGridFsBucket());

builder.Services.AddCors(CorsConfig.CorsPolicyConfig);

builder.Services.AddSingleton<JwtHandler>(); // jwt auth

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
builder.Services.AddTransient<TokenBlacklistMiddleware>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SchemaFilter<DateOnlySchemaFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseCors(CorsConfig.CorsPolicyName);

app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();