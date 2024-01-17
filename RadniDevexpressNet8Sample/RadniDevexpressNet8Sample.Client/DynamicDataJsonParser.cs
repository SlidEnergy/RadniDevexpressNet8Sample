using System.ComponentModel;
using System.Dynamic;
using CommonBlazor.DynamicData.Models;
using Newtonsoft.Json;

namespace CommonBlazor.DynamicData
{
    public static class DynamicDataJsonParser
    {
        public static IEnumerable<ExpandoObject> ParseList(string json, IEnumerable<GenericColumnSettings> properties)
        {
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json) ?? new List<Dictionary<string, object>>();

            return list.Select(x => ConvertDictionaryToModel(x, ConvertPropertiesToDictionary(properties)));
        }

        public static ExpandoObject ParseModel(string json, IEnumerable<GenericColumnSettings> properties)
        {
            var model = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            return ConvertDictionaryToModel(model, ConvertPropertiesToDictionary(properties));
        }

        private static Dictionary<string, GenericColumnSettings> ConvertPropertiesToDictionary(IEnumerable<GenericColumnSettings> properties)
        {
            return properties.ToDictionary(x => x.FullPropertyName.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString(), x => x);
        }

        private static ExpandoObject ConvertDictionaryToModel(Dictionary<string, object> model, Dictionary<string, GenericColumnSettings> properties)
        {
            var dataRow = new ExpandoObject() as IDictionary<string, object?>;

            if (dataRow == null)
                throw new ArgumentOutOfRangeException(nameof(model));

            foreach (var key in model.Keys)
            {
                var value = model[key];

                if (value == null || value is DateTime)
                {
                    dataRow.Add(key, value);
                    continue;
                }

                if (!properties.TryGetValue(key, out var prop))
                    continue;

                var type = prop.ParsedType;

                if (type.IsNullable())
                {
                    var args = type.GetGenericArguments();
                    if (args.Count() > 0)
                        type = args[0];
                }

                if (type.IsNumeric())
                {
                    if (type.IsIntegral())
                    {
                        value = Convert.ChangeType((long)value, type);
                    }
                    else if (type.IsFloatingPoint())
                    {
                        value = Convert.ChangeType((double)value, type);
                    }

                    dataRow.Add(key, value);
                }
                else if (type.IsEnum)
                {
                    dataRow.Add(key, Enum.ToObject(type, value));
                }
                else
                {
                    dataRow.Add(key, Convert.ChangeType(value, type));
                }


                //if (type != null)
                //{
                //    var underlyingType = Nullable.GetUnderlyingType(type);

                //    if (underlyingType != null)
                //    {
                //        if (value != null)
                //        // TODO: whatever here value is not nullable type
                //        value = Activator.CreateInstance(type, Convert.ChangeType(value, underlyingType));
                //    }
                //    else
                //        value = Convert.ChangeType(value, type);
                //}

                //dataRow.Add(key, value);
            }

            return (ExpandoObject)dataRow!;
        }
    }
}
