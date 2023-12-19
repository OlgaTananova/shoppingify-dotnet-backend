using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using shoppingify_backend.Helpers;
using shoppingify_backend.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// Add AuthContext to connect to identity database
builder.Services.AddDbContext<AuthContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add ApplicationContext 
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add Identity Provider  + requirements for user's passwords
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
})
    .AddEntityFrameworkStores<AuthContext>()
    .AddDefaultTokenProviders();

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", builder => {
        builder.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Add Controllers
builder.Services.AddControllers();


// Add Authentication + HttpOnly Cookie
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

// In case using a custom JWT Token
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
//            ValidateIssuer = true,
//            ValidIssuer = builder.Configuration["JWT:Issuer"],
//            ValidateAudience = true,
//            ValidAudience = builder.Configuration["JWT:Audience"],
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero
//        };
//        // Setup extraction of a token from cookies
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                var tokenName = builder.Configuration["JWT:TokenName"]?? "Token";
//                if (context.Request.Cookies.ContainsKey(tokenName))
//                {
//                    context.Token = context.Request.Cookies[tokenName];
//                }
//                return Task.CompletedTask;
//            }
//        };
//    }
//);


// Built-in token sent via httponly cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;  // Make the cookie HTTP-only
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Enforce HTTPS
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Auth/login";
    options.LogoutPath = "/Auth/logout";
    options.Cookie.Name = "Token";
    options.SlidingExpiration = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

//Logging middleware
app.UseMiddleware<ErrorLoggingMiddleware>();

// Cors
app.UseCors("AllowLocal");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
