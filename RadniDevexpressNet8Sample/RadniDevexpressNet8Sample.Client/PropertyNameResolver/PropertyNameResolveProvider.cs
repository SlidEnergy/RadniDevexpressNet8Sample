using CommonBlazor.DynamicData.Models;

namespace CommonBlazor.DynamicData
{
    public class PropertyNameResolveProvider : IPropertyNameResolveProvider
    {
        public PropertyNameResolver GetUpperCasePropertyNameResolver(List<GenericColumnSettings> properties)
        {
            var distinctList = properties.GroupBy(x => x.FullPropertyName).Select(x => x.First());
            var _propertiesDict = distinctList.ToDictionary(x => x.FullPropertyName.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString());

            return new PropertyNameResolver()
            {
                Resolver = (propertyName) =>
                {
                    if (!_propertiesDict.ContainsKey(propertyName))
                        throw new ArgumentOutOfRangeException($"Property with PropertyName: {propertyName} don't exist in list of properties");

                    return _propertiesDict[propertyName].FullPropertyName ?? propertyName;
                }
            };
        }
    }
}
