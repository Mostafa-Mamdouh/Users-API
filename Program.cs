using API.Errors;
using API.Middleware;
using Application.Helpers;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using UserAPI.Data;
using UserAPI.Dto;
using UserAPI.Interfaces;
using UserAPI.Services;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Users API", Version = "v1", Description = ".Net Core 7 Minimal API" });
    var securitySchema = new OpenApiSecurityScheme
    {

        Description = "JWT Auth Bearer Scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } };
    c.AddSecurityRequirement(securityRequirement);
});
#endregion

#region DB Connection
// DB Connection
IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();


builder.Services.AddDbContext<DBUserContext>(x =>
{
    x.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});
#endregion

#region IOC
// IOC
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddTransient<ITokenService, TokenService>();
#endregion

#region Security
// JWT Token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
          ValidateIssuer = false,
          ValidateAudience = false,
      };
  });

//Cors

builder.Services.AddCors(async opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHsts();
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();


#region End Points

app.MapPost("/user", [AllowAnonymous]
async (RequesDto user, DBUserContext context, ITokenService tokenService, IMapper mapper, HttpResponse response) =>
{
    var userData = mapper.Map<RequesDto, User>(user);
    await context.Users.AddAsync(userData);
    await context.SaveChangesAsync();
    await response.WriteAsJsonAsync(new TokenDto() { Id = userData.Id, AccessToken = tokenService.GenerateAccessToken() });
});

app.MapGet("/user", [Authorize(AuthenticationSchemes = "Bearer")]
async (string id, DBUserContext context, IMapper mapper, HttpResponse response) =>
{
    var userData = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
    if (userData == null)
        await response.WriteAsJsonAsync(Results.BadRequest("User Not Found."));

    await response.WriteAsJsonAsync(mapper.Map<User, UserDto>(userData));

});

app.Map("errors/{code}", async (int code, HttpResponse response)
=>
{
    await response.WriteAsJsonAsync(new ObjectResult(new ApiResponse(code)));
}).WithMetadata(new ApiExplorerSettingsAttribute { IgnoreApi = true });
#endregion



// For dynamic update database against migrations
using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DBUserContext>();
    await context.Database.MigrateAsync();
}


app.Run();

