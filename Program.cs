using project_core.Services;
using project.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using project_core.Middlewares;
using Serilog;
using Serilog.Events; // Add this line


var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // תיקייה log-.txt עם קובץ חדש כל יום
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KsPizza", Version = "v1" });
});
builder.Services.AddPizzaService();
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

app.UseAuthorization();

app.MapControllers();

app.Run();