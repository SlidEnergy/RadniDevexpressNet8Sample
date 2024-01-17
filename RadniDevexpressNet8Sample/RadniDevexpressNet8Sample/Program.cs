using CommonBlazor.Infrastructure;
using RadniDevexpressNet8Sample.Client.Pages;
using RadniDevexpressNet8Sample.Components;
using RadniDevexpressNet8Sample.Data;
using System.Globalization;
using AndromedaBlazor.Api;
using CommonBlazor.DynamicData;
using CommonBlazor.UI;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var httpClientOptions = builder.Configuration.GetSection(nameof(HttpClientOptions)).Get<HttpClientOptions>();

ApiCollection.Items = new[]
{
    new ApiCollection.Api
    {
        Name = "Authentication",
        Url = httpClientOptions.ApiCollection.First(y=>y.Name=="Authentication").BaseAddress
    },
    new ApiCollection.Api
    {
        Name = "MyApplicationApi",
        Url = httpClientOptions.ApiCollection.First(y=>y.Name=="MyApplicationApi").BaseAddress
    }
};

if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("DevelopmentWithRemoteServer"))
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

builder.Services.AddOptions();
builder.Services.AddSingleton<HttpClientManager>();
//builder.Services.AddDispatcher();
//builder.Services.AddTransient<SsrNavigationService>();
//builder.Services.AddTransient<JsUtils>();
builder.Services.AddInfrastructure();
//builder.Services.AddLocalizationServices(builder.Configuration);
//builder.Services.AddAuthentication(builder.Configuration);

//builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, BlazorAuthorizationMiddlewareResultHandler>();


//builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddAuthentication(options =>
//    {
//        //options.DefaultAuthenticateScheme = Constants.AuthenticationScheme.AuthenticationScheme;
//        //options.DefaultChallengeScheme = Constants.AuthenticationScheme.BearerScheme;
//        //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultScheme = IdentityConstants.BearerScheme;
//        options.DefaultSignInScheme = IdentityConstants.BearerScheme;
//    })
//    .AddIdentityCookies();

builder.Services.AddSingleton<ApplicationApiClient>();
//builder.Services.AddValidators();

var dynamicOptions = builder.Configuration.GetSection(nameof(DynamicDataProviderOptions)).Get<DynamicDataProviderOptions>();
builder.Services.AddDynamicData(opts =>
{
    opts.BaseAddress = dynamicOptions.BaseAddress;
    opts.InitializationEndpoint = "genericEndpoint/initialize";
    opts.SaveColumnsEndpoint = "genericEndpoint/columns";
    opts.QueryEndpoint = "genericEndpoint/query";
    opts.GetCountEndpoint = "genericEndpoint/count";
    opts.AvailableColumnsEndpoint = "genericEndpoint/availableColumns";
    opts.AvailableCollectionsEndpoint = "genericEndpoint/availableCollections";
    opts.ClientId = "myClientId";
});


//builder.Services.AddSingleton<ApplicationApiClient>();
//TODO Check why this is not work, options not working
builder.Services.AddUI(options => {
//    options.GridVirtualScrollEnabled = true;
});
//builder.Services.AddUIJs();
//builder.Services.AddExceptionHandling(builder.Logging);

builder.Services.AddDevExpressBlazor(options =>
{
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
builder.Services.AddSingleton<WeatherForecastService>();

//builder.Services.AddSingleton<IPermissionService, PermissionService>();

var culture = new CultureInfo("hr-HR");
//var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = culture;

var app = builder.Build();

ServiceResolver.ServiceProvider = app.Services;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
