using CommonBlazor.Infrastructure;
using CommonBlazor.UI;
using CommonBlazor.UI.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;
using AndromedaBlazor.Api;
using CommonBlazor.DynamicData;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

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

if (builder.HostEnvironment.IsDevelopment() || builder.HostEnvironment.IsEnvironment("DevelopmentWithRemoteServer"))
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

builder.Services.AddOptions();
builder.Services.AddInfrastructure();
//builder.Services.AddLocalizationServices(builder.Configuration);
//builder.Services.AddAuthentication(builder.Configuration);
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

builder.Services.AddSingleton<ApplicationApiClient>();
//TODO Check why this is not work, options not working

builder.Services.AddUI(options => {
    options.GridVirtualScrollEnabled = true;
});
builder.Services.AddTransient<JsUtils>();
//builder.Services.AddUIJs();
//builder.Services.AddExceptionHandling(builder.Logging);


builder.Services.AddDevExpressBlazor(options =>
{
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});

//builder.Services.AddSingleton<IPermissionService, PermissionService>();

var culture = new CultureInfo("hr-HR");
//var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = culture;

var host = builder.Build();
//host.Services.UseAuthentication();

ServiceResolver.ServiceProvider = host.Services;

//will be removed when configure service will work
var commonGlobal = ServiceResolver.Resolve<GlobalCommonConfiguration>();
commonGlobal.GridVirtualScrollEnabled = true;
 await host.RunAsync();
