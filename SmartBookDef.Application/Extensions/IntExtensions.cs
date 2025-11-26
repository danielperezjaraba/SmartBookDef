namespace SmartBookDef.Application.Extensions;
public static class IntExtensions
{
    public static int CalcularEdad(this DateTime fechaNacimiento)
    {

        DateTime fechaActual = DateTime.Today;
        int edad = fechaActual.Year - fechaNacimiento.Year;

        if (fechaNacimiento.Date > fechaActual.AddYears(-edad))
        {
            edad--;
        }
        return edad;


    }
}
