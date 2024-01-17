using CommonBlazor.DynamicData.Models;

namespace CommonBlazor.DynamicData
{
    public interface IPropertyNameResolveProvider
    {
        PropertyNameResolver GetUpperCasePropertyNameResolver(List<GenericColumnSettings> properties);
    }
}