using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
//using Blazored.LocalStorage;
//using Blazored.SessionStorage;
using CommonBlazor.Infrastructure.Dispatcher;

namespace CommonBlazor.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.Configure<JsonSerializerOptions>((options) => options.PropertyNameCaseInsensitive = true);

            services.AddSingleton<HttpClientManager>();
            //services.AddScoped<BrowserStorageService>();

            foreach (var api in ApiCollection.Items)
            {
                services.AddSingleton(sp =>
                {
                    //var delegatingHandlerProvider = sp.GetRequiredService<IHttpClientDelegatingHandlerProvider>();
                    var httpClientModel = new ApiHttpClientModel(api.Name);//, delegatingHandlerProvider);

                    return httpClientModel;
                });
            }

            services.Configure<HttpClientOptions>(x => x.ApiCollection = ApiCollection.Items.Select(x => new HttpClientOptions.ApiOptions
            {
                BaseAddress = x.Url,
                Name = x.Name
            }).ToArray());

            //services.AddBlazoredLocalStorage();
            //services.AddBlazoredSessionStorage();

            services.AddDispatcher();

            services.AddSingleton<ApplicationCache>();

            return services;
        }

        private static IServiceCollection AddDispatcher(this IServiceCollection services, IEnumerable<Type> applicationTypes = null)
        {
            if (applicationTypes is null)
                applicationTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

            var requests = applicationTypes.GetGenericInterfaceImplementingTypes(typeof(IRequest<>));
            var requestHandlerTypes = GetRequestHandlerTypes(applicationTypes);

            services.AddSingleton(x => new DispatcherCache(requestHandlerTypes, requests));
            services.AddScoped<IDispatcher, DispatcherService>();

            return services;
        }

        private static Type[] GetRequestHandlerTypes(IEnumerable<Type> applicationTypes)
        {
            var requestHandlerTypes = applicationTypes.GetGenericInterfaceImplementingTypes(typeof(IRequestHandler<,>));
            var handlerOverrides = requestHandlerTypes
                .GroupBy(x => x.GetInterfaces().FirstOrDefault(y => y.IsGenericType).GetGenericArguments().First().TypeHandle)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.ToArray());

            var overridenHandlers = handlerOverrides.Where(x => !typeof(IHandlerOverride).IsAssignableFrom(x)).Select(x => x.TypeHandle);
            requestHandlerTypes = requestHandlerTypes.Where(x => !overridenHandlers.Contains(x.TypeHandle)).ToArray();

            return requestHandlerTypes;
        }

        public static object ConstructInstance(this IServiceProvider serviceProvider, Type instanceType)
        {
            return CreateInstance(serviceProvider, instanceType);
        }

        public static Type[] GetGenericInterfaceImplementingTypes(this IEnumerable<Type> typeCollection, Type genericInterfaceType)
        {
            return typeCollection.Where(x => !x.IsInterface && !x.IsAbstract &&
               x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == genericInterfaceType)).ToArray();
        }

        private static object CreateInstance(IServiceProvider serviceProvider, Type instanceType)
        {
            var constructor = instanceType.GetConstructors().SingleOrDefault();

            var parameters = !(constructor is null)
                ? constructor.GetParameters()
                : Array.Empty<ParameterInfo>();

            var services = new List<object>();
            foreach (var parameter in parameters)
            {
                services.Add(serviceProvider.GetService(parameter.ParameterType));
            }

            return Activator.CreateInstance(instanceType, services.ToArray());
        }
    }
}
