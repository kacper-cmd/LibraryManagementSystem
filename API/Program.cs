using API.Extensions;
using API.Filters;
using FluentValidation.AspNetCore;
using Application.ValidatorsDto;
using API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using Microsoft.Extensions.Configuration;
using Serilog.Settings.Configuration;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureCorsPolicy();

builder.Services.ConfigureDatabase(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    };
});

#region Validators



builder.Services.
    AddFluentValidationAutoValidation().
    AddFluentValidationClientsideAdapters();
//depreciated
//builder.Services.AddControllers()
//.AddFluentValidation(fv =>
//  fv.RegisterValidatorsFromAssemblyContaining<BookDtoValidator>());

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Add FluentValidation validator
builder.Services.ConfigureApplication();
#endregion



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LibraryManagmentSystem", Version = "v1" });
    c.DocumentFilter<JsonPatchDocumentFilter>();
});


#region Configure Serilog

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.File("logs/log.txt",
		rollingInterval: RollingInterval.Day,
		rollOnFileSizeLimit: true)
	.CreateLogger();

builder.Host.UseSerilog(Log.Logger);

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
	app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseMiddleware(typeof(GlobalErrorHandling));


app.UseHttpsRedirection();
app.UseCors("CorsPolicy");


app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();

