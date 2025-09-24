using Data.Contexts;
using Business.Interfaces;
using Business.Services;

using Microsoft.EntityFrameworkCore;
using Data.Interfaces;
using Data.Repositories;
using Business.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<DataContext>();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

//Här lägger vi repositories
builder.Services.AddScoped<ISessionRepository, SessionRespository>();

//Här lägger vi services
builder.Services.AddScoped<ISessionService, SessionService>();

var app = builder.Build();

app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GymSlay API V.0");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();
app.UseCors( x =>
    x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
);

app.UseAuthorization();

app.MapControllers();

app.Run();