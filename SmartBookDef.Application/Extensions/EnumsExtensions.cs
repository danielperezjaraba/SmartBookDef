namespace SmartBookDef.Application.Extensions;
public static class EnumsExtensions
{
    public static TEnum ToEnum<TEnum>(this int intValue) where TEnum : struct, Enum
    {
        if (Enum.IsDefined(typeof(TEnum), intValue))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), intValue);
        }
        throw new ArgumentException($"El valor {intValue} no es válido, utilice 1 o 2.");
    }
}
