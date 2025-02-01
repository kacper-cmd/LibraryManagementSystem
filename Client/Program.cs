using Application.ValidatorsDto;
using Client.Components;
using Client.Helpers;
using Client.Services;
using Client.States;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7018/api/") });

#region HttpClient
var apiUrl = builder.Configuration["ApiUrl"] ?? URLConst.apiUrl;
builder.Services.AddHttpClient("client", httpClient =>
{
    httpClient.BaseAddress = new Uri(apiUrl);
});

#endregion

#region Services
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IRequestHelper, RequestHelper>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExcelService,ExcelService>();
builder.Services.AddScoped<IFontLoader,FontLoader>();
builder.Services.AddScoped<IPdfService, PdfService>();
#endregion

var app = builder.Build();

string[] supportedCultures = ["en-US", "pl-PL"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
