namespace DomusApi.Helpers.Enums
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumTypeAttribute : Attribute
    {
        public Type Type { get; }

        public EnumTypeAttribute(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
