using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.DataAnotations;
public class CorreoCecarAttribute : RegularExpressionAttribute
{
    public CorreoCecarAttribute()
        : base(@"^[^@\s]+@cecar\.edu\.co$")
    {
        ErrorMessage = "El correo debe pertenecer al dominio @cecar.edu.co.";
    }
}