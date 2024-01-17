using System.Text.RegularExpressions;

namespace CommonBlazor.DynamicData
{
    public static class TypeUtils
    {
        public static Type? GetType(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
                return null;

            var type = Type.GetType(fullTypeName);

            if (type != null)
                return type;

            type = FindAndGetType(fullTypeName);

            if (type != null)
                return type;

            type = FindNullableType(fullTypeName);

            if(type != null) 
                return type;

            type = FindAndGetTypeByTypeName(fullTypeName.Split('.').Last());

            if (type != null) 
                return type;

            return null;
        }


        static string GetNotNullableTypeName(string fullTypeName)
        {
            var regex = new Regex(@"System.Nullable`1\[\[(?<type>[^,\]]+),.+\]\]"); //System.Nullable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]

            var matches = regex.Match(fullTypeName);
            var notNullableTypeName = matches.Groups["type"].Value;

            return notNullableTypeName;
        }

        public static Type? FindNullableType(string fullTypeName)
        {
            var notNullableTypeName = GetNotNullableTypeName(fullTypeName);

            if (string.IsNullOrEmpty(notNullableTypeName))
                return null;

            var type = Type.GetType(notNullableTypeName);

            if (type != null)
                return CreateNullable(type);

            type = FindAndGetType(notNullableTypeName);

            if (type != null)
                return CreateNullable(type);

            type = FindAndGetTypeByTypeName(notNullableTypeName.Split('.').Last());

            if (type != null)
                return CreateNullable(type);

            return null;
        }

        public static Type? CreateNullable(Type type)
        {
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);

            // Is already nullable
            return type;
        }

        public static Type? FindAndGetTypeByTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if(type.Name == typeName) 
                        return type;
                }
            }

            return null;
        }

        public static Type? FindAndGetType(string fullTypeName)
        {
            if(string.IsNullOrEmpty(fullTypeName))
                return null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);

                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
