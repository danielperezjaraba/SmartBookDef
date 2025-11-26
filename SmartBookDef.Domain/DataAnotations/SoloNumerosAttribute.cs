using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.DataAnotations;
public class SoloNumerosAttribute : RegularExpressionAttribute
{
    public SoloNumerosAttribute()
        : base(@"^\d+$")
    {
        ErrorMessage = "La identificación solo puede contener números.";
    }
}