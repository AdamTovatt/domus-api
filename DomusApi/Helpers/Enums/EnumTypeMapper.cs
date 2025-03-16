using System.Collections.Concurrent;
using System.Reflection;

namespace DomusApi.Helpers.Enums
{
    public class EnumTypeMapper<T>
            where T : struct, Enum
    {
        private static readonly ConcurrentDictionary<T, Type> enumToTypeCache = new();
        private static readonly ConcurrentDictionary<Type, T> typeToEnumCache = new();

        static EnumTypeMapper()
        {
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                Type? typeValue = GetTypeValue(enumValue);
                if (typeValue != null)
                {
                    enumToTypeCache[enumValue] = typeValue;
                    typeToEnumCache[typeValue] = enumValue;
                }
            }
        }

        private static Type? GetTypeValue(T enumValue)
        {
            FieldInfo? field = typeof(T).GetField(enumValue.ToString());
            EnumTypeAttribute? attribute = field?.GetCustomAttributes(typeof(EnumTypeAttribute), false).FirstOrDefault() as EnumTypeAttribute;

            return attribute?.Type;
        }

        public static Type? GetEnumType(T theEnum)
        {
            return enumToTypeCache.TryGetValue(theEnum, out Type? typeValue) ? typeValue : null;
        }

        public static T? GetEnumFromType(Type type)
        {
            return typeToEnumCache.TryGetValue(type, out T enumValue) ? enumValue : null;
        }
    }
}
