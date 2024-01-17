using CommonBlazor.Infrastructure;
using CommonBlazor.UI.Components;
using CommonBlazor.UI.Configuration;
using CommonBlazor.UI.Filtering;
using CommonBlazor.UI.List;
using CommonBlazor.UI.Shared;
using DevExpress.Blazor.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommonBlazor.UI
{
    public static class UIExtensions
    {
        public static IServiceCollection AddUI(this IServiceCollection services, Action<GlobalCommonConfiguration> configure = null)
        {
            services.AddSingleton<GlobalCommonConfiguration>();
            //services.AddTransient<FilteringState>();
            services.AddScoped<ListState>();
            //services.AddSingleton<SideDrawerState>();
            //services.AddTransient<UICommandManager>();
            services.AddTransient<IFilteringService, FilteringService>();
            //services.AddScoped<IToastService, ToastService>();
            //services.AddScoped<BusyIndicatorService>();
            //services.AddTransient<NavigationService>();
            //services.AddSingleton<SideDrawerService>();
            //services.AddSingleton<DialogService>();
            services.AddSingleton<IMessenger, Messenger>();

            if (configure != null)
            {
                services.Configure(configure);
            }

            return services;
        }

        public static IServiceCollection AddUIJs(this IServiceCollection services)
        {
            services.AddTransient<JsUtils>();
            //services.AddTransient<JsDxFilterBuilder>();
            //services.AddTransient<JsDxLoadIndicator>();
            //services.AddTransient<JsDxLoadPanel>();
            //services.AddTransient<JsDxSelectBox>();
            //services.AddTransient<JsDxTreeList>();
            //services.AddTransient<JsDxColorBox>();
            //services.AddTransient<JsIntersectionObserver>();
            //services.AddTransient<JsValidator>();

            return services;
        }

        //public static IServiceCollection AddExceptionHandling(this IServiceCollection services, ILoggingBuilder loggingBuilder)
        //{
        //    services.AddTransient<ExceptionHandler>();

        //    var unhandledExceptionSender = new UnhandledExceptionSender();
        //    var myLoggerProvider = new UnhandledExceptionProvider(unhandledExceptionSender);
        //    loggingBuilder.AddProvider(myLoggerProvider);
        //    services.AddSingleton<IUnhandledExceptionSender>(unhandledExceptionSender);

        //    return services;
        //}
    }
}
