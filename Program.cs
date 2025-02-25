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


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.TokenValidationParameters = TokenService.GetTokenValidationParameters();
    });
//nowwwww
// builder.Services.AddAuthorization(cfg =>
//     {
//         cfg.AddPolicy("Admin", policy => policy.RequireClaim("isAdmin", "Admin"));
//         cfg.AddPolicy("User", policy => policy.RequireClaim("isAdmin", "User", "Admin"));
//     });

builder.Services.AddAuthorization(cfg =>
    {
        cfg.AddPolicy("Admin", policy => policy.RequireClaim("isAdmin", "true"));
        cfg.AddPolicy("User", policy => policy.RequireClaim("isAdmin", "true", "false"));
    });


//     services.AddAuthorization(options =>
// {
//     options.AddPolicy("Admin", policy => policy.RequireClaim("isAdmin", "Admin"));
//     options.AddPolicy("User", policy => policy.RequireClaim("isAdmin", "User", "Admin"));
// });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // תיקייה log-.txt עם קובץ חדש כל יום
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

app.UseLoggerMiddlewates();
app.UseErrorHandlingMiddleware();
/*js*/
app.UseDefaultFiles();
app.UseStaticFiles();
/*js (remove "launchUrl" from Properties\launchSettings.json*/

app.UseHttpsRedirection();
app.UseAuthentication();  // ✅ הוספנו את זה לפני Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();



