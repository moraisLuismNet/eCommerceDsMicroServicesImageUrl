using CdService.AutoMappers;
using CdService.DTOs;
using CdService.Models;
using CdService.Repository;
using CdService.Services;
using CdService.Validators;


using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = null
});

// Disable static web assets to prevent wwwroot lookup
builder.WebHost.UseStaticWebAssets();

// Add services to the container.
builder.Services.AddDbContext<CdContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CdDBConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
});

// Configure security
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(builder.Configuration["JWTKey"]))
               });

// Setting up security in Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authentication Using Bearer Scheme. \r\n\r " +
        "Enter the word 'Bearer' followed by a space and the authentication token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

});

//CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Validators
builder.Services.AddScoped<IValidator<MusicGenreInsertDTO>, MusicGenreInsertValidator>();
builder.Services.AddScoped<IValidator<MusicGenreUpdateDTO>, MusicGenreUpdateValidator>();
builder.Services.AddScoped<IValidator<RecordInsertDTO>, RecordInsertValidator>();
builder.Services.AddScoped<IValidator<RecordUpdateDTO>, RecordUpdateValidator>();
builder.Services.AddScoped<IValidator<GroupInsertDTO>, GroupInsertValidator>();
builder.Services.AddScoped<IValidator<GroupUpdateDTO>, GroupUpdateValidator>();

// Services

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IMusicGenreService, MusicGenreService>();
builder.Services.AddScoped<IRecordService, RecordService>();

builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7170/"); 
});

// Mappers
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repositories
builder.Services.AddScoped<IGroupRepository<Group>, GroupRepository>();
builder.Services.AddScoped<IMusicGenreRepository<MusicGenre>, MusicGenreRepository>();
builder.Services.AddScoped<IRecordRepository<Record>, RecordRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
