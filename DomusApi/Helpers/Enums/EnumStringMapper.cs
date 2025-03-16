using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;

namespace DomusApi.Helpers.Enums
{
    public class EnumStringMapper<T>
        where T : struct, Enum
    {
        private static readonly ConcurrentDictionary<T, string> enumToDatabaseValueCache = new ConcurrentDictionary<T, string>();
        private static readonly ConcurrentDictionary<string, T> stringValueToEnumCache = new ConcurrentDictionary<string, T>();

        static EnumStringMapper()
        {
            // Populate the caches
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                string stringValue = GetStringValue(enumValue);

                enumToDatabaseValueCache[enumValue] = stringValue;
                stringValueToEnumCache[stringValue] = enumValue;
            }
        }

        private static string GetStringValue(T enumValue)
        {
            FieldInfo? field = typeof(T).GetField(enumValue.ToString());
            EnumMemberAttribute? attribute = field?.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() as EnumMemberAttribute;

            if (attribute == null)
                throw new InvalidOperationException($"The enum value '{enumValue}' of type '{typeof(T).Name}' is missing a [PgName] attribute.");

            if (attribute.Value == null)
                throw new InvalidDataException($"The enum value '{enumValue}' of tpe '{typeof(T).Name}' is missing a defined value for the EnumMemberAttribute that is on it.");

            return attribute.Value;
        }

        public static string GetAsString(T theEnum)
        {
            return enumToDatabaseValueCache[theEnum];
        }

        public static string? GetAsString(T? theEnum)
        {
            if (theEnum == null) return null;
            return enumToDatabaseValueCache[theEnum.Value];
        }

        public static T? GetNullableEnum(string? stringValue)
        {
            if (string.IsNullOrEmpty(stringValue)) return null;
            return GetEnum(stringValue);
        }

        public static T GetEnum(string stringValue)
        {
            if (stringValueToEnumCache.TryGetValue(stringValue, out T enumValue)) return enumValue;
            throw new ArgumentException($"The value '{stringValue}' is not valid for enum type '{typeof(T).Name}'.");
        }

        public static T? GetNullableEnum(int? nullableIntValue)
        {
            if (nullableIntValue == null) return null;
            return GetEnum(nullableIntValue.Value);
        }

        public static T GetEnum(int intValue)
        {
            if (!Enum.IsDefined(typeof(T), intValue))
                throw new ArgumentException($"The value '{intValue}' is not valid for enum type '{typeof(T).Name}'.");

            return (T)Enum.ToObject(typeof(T), intValue);
        }
    }
}
