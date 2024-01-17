using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace CommonBlazor.Infrastructure
{
    public static class ServiceResolver
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static IServiceProvider ServerServiceProvider { get; set; }

        private static Dictionary<string, IServiceScope> ScopeCollection { get; set; }

        static ServiceResolver()
        {
            ScopeCollection = new Dictionary<string, IServiceScope>();
        }

        public static T Resolve<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public static T ResolveSsr<T>()
        {
            return ServerServiceProvider.GetService<T>();
        }

        public static T Resolve<T>(string scopeId)
        {
            if (ScopeCollection.ContainsKey(scopeId))
                return ScopeCollection[scopeId].ServiceProvider.GetService<T>();
            else
                return default(T);
        }

        public static void CreateScope(string id)
        {
            if (!ScopeCollection.ContainsKey(id))
            {
                var scope = ServiceProvider.CreateScope();

                ScopeCollection.Add(id, scope);
            }
        }

        public static void DisposeScope(string id)
        {
            if (ScopeCollection.TryGetValue(id, out IServiceScope scope))
            {
                ScopeCollection.Remove(id);
                scope.Dispose();
            }
        }
    }
}
