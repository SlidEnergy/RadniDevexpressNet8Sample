using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonBlazor.DynamicData
{
    public class DynamicEntities : IDynamicEntities
    {
        private readonly IDictionary<string, IDynamicEntityService> _entityServices = new Dictionary<string, IDynamicEntityService>();

        public IDynamicEntityService GetOrCreateEntityService(DynamicEntityContext entityContext)
        {
            if (entityContext == null || string.IsNullOrEmpty(entityContext.EntityName))
                throw new ArgumentNullException(nameof(entityContext));

            if (!_entityServices.ContainsKey(entityContext.EntityName))
            {
                var type = typeof(DynamicEntityService<>).MakeGenericType(entityContext.KeyFieldType);

                var entityService = (IDynamicEntityService)Activator.CreateInstance(type, entityContext);

                _entityServices.Add(entityContext.EntityName, entityService);

                return entityService;
            }
            else
                return _entityServices[entityContext.EntityName];
        }

        public bool TryGetEntityService(string entityName, out IDynamicEntityService entityService)
        {
            return _entityServices.TryGetValue(entityName, out entityService);
        }
    }
}
