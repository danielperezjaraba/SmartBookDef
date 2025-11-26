using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.DataAnotations;
public class CelularDiezDigitosAttribute : RegularExpressionAttribute
{
    public CelularDiezDigitosAttribute()
        : base(@"^\d{10}$")
    {
        ErrorMessage = "El celular debe tener exactamente 10 dígitos.";
    }
}
