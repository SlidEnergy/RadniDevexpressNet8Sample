namespace CommonBlazor.DynamicData;

public interface IDynamicEntities
{
    IDynamicEntityService GetOrCreateEntityService(DynamicEntityContext entityContext);
    bool TryGetEntityService(string entityName, out IDynamicEntityService entityService);
}