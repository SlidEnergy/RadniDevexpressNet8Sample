using System.Dynamic;

namespace CommonBlazor.Extensions
{
    public static class ExpandoObjectExtentions
    {
        public static bool TryGetValue(this ExpandoObject obj, string key, out object value)
        {
            return (obj as IDictionary<string, object>).TryGetValue(key, out value);
        }

        public static object GetValue(this ExpandoObject obj, string key)
        {
            return (obj as IDictionary<string, object>)[key];
        }

        public static T ConvertTo<T>(this ExpandoObject obj, bool propertyNameLower) where T : class
        {
            if (typeof(T) == typeof(ExpandoObject))
                return obj.CastTo<T>();

            var type = typeof(T);

            var model = Activator.CreateInstance(type);

            if (model == null)
                throw new Exception("Can't create a model of type " + type);

            var props = type.GetProperties();

            var dict = obj as IDictionary<string, object>;

            foreach (var prop in props)
            {
                var propName = propertyNameLower ? prop.Name.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString() : prop.Name;

                if (dict.ContainsKey(propName))
                    prop.SetValue(model, dict[propName]);
            }

            return (T)model;
        }
    }
}
