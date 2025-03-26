using project_core.Services;
using project.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using project_core.Middlewares;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(options =>
    {

        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.TokenValidationParameters = TokenService.GetTokenValidationParameters();
    })


   .AddCookie(options =>
{
        options.Cookie.SameSite = SameSiteMode.None; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.LoginPath = "/account/google-login";
        options.AccessDeniedPath = "/account/access-denied";
})

    .AddGoogle(options =>
    {
        options.ClientId = "615496630298-8lb43opj1vhse725dql9aucvh1369th0.apps.googleusercontent.com";
        options.ClientSecret = "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";
        options.CallbackPath = "/signin-google"; 
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Scope.Add("email"); 
        options.Scope.Add("profile"); 
    });

builder.WebHost.ConfigureKestrel(options =>
{
        options.ListenAnyIP(5073);
        options.ListenAnyIP(5074, listenOptions =>
        {
            listenOptions.UseHttps();
        });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod() 
              .AllowAnyHeader();

    });
});

builder.Services.AddAuthorization(cfg =>
    {
        cfg.AddPolicy("Admin", policy => policy.RequireClaim("isAdmin", "true"));
        cfg.AddPolicy("User", policy => policy.RequireClaim("isAdmin", "true", "false"));
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Book", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter JWT with Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                },
            new string[] {}
        }
        });
    });
Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddBookService();
builder.Services.AddUserService();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseLoggerMiddlewates();
app.UseErrorHandlingMiddleware();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self' https://accounts.google.com");
    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();



