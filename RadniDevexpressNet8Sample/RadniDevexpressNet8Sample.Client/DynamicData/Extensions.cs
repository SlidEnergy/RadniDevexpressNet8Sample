using Microsoft.Extensions.DependencyInjection;
using Common.Filtering.Utils;
using CommonBlazor.DynamicData.Abstractions;
using CommonBlazor.DynamicData.Filtering;

namespace CommonBlazor.DynamicData
{
    public static class Extensions
    {
        public static IServiceCollection AddDynamicData(this IServiceCollection services, Action<DynamicDataProviderOptions> configureOptions)
        {
            services.AddSingleton<DynamicEntityContextInitializer>();
            services.Configure<DynamicDataProviderOptions>(configureOptions);
            services.AddTransient<IDynamicDataProvider, DynamicDataProvider>();
            //services.AddTransient(typeof(IEntityDataProvider<>), typeof(EntityDataProvider<>));
            //services.AddTransient<DynamicEntityAvailableColumnsService>();
            //services.AddTransient<DynamicEntityAvailableCollectionsService>();
            services.AddTransient<IPropertyNameResolveProvider, PropertyNameResolveProvider>();

            services.Configure<CustomFiltersProviderOptions>(x =>
            {
                x.GetAll = "sys/customFilters";
                x.Create = "sys/customFilters/create";
                x.Delete = "sys/customFilters";
                x.Save = "sys/customFilters";
                x.GetById = "sys/customFilters";
            });

            //services.Configure<QuickFiltersProviderOptions>(x =>
            //{
            //    x.GetAll = "sys/quickFilters";
            //    x.Create = "sys/quickFilters/create";
            //    x.Delete = "sys/quickFilters";
            //    x.Save = "sys/quickFilters";
            //    x.GetById = "sys/quickFilters";
            //});

            services.AddTransient<ICustomFiltersProvider, CustomFiltersProvider>();
            //services.AddTransient<IQuickFiltersProvider, QuickFiltersProvider>();

            services.AddTransient<IFilterCriteriaToDevextremeConverter, FilterCriteriaToDevextremeConverter>();

            services.AddSingleton<IDynamicEntities, DynamicEntities>();

            return services;
        }
    }
}
