using Data.Contexts;
using Business.Interfaces;
using Business.Services;
using Microsoft.EntityFrameworkCore;
using Data.Interfaces;
using Data.Repositories;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
//builder.Services.AddScoped<DataContext>();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddIdentity<UserEntity, IdentityRole<Guid>>(options => 
{
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;

})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/login";
    options.AccessDeniedPath = "/api/auth/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
});


//Här lägger vi repositories
builder.Services.AddScoped<ISessionRepository, SessionRespository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

//Här lägger vi services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUserService, UserService>();

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
    x.WithOrigins("http://localhost:5173", "https://localhost:7067").AllowAnyMethod().AllowAnyHeader().AllowCredentials()
);


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();