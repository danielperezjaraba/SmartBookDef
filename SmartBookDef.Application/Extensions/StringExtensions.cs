using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SmartBookDef.Application.Extensions;
public static class StringExtensions
{
    public static string Sanitize(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var decoded = HttpUtility.HtmlDecode(input);
        string clean = Regex.Replace(decoded, "<.*?>", string.Empty);

        clean = Regex.Replace(clean, "(?i)script|onerror|onload|alert|eval|javascript", string.Empty);

        return clean.Trim();
    }

    public static string RemoveAccents(this string texto)
    {
        var acentos = new Dictionary<char, char>
        {
            { 'á', 'a' }, { 'é', 'e' }, { 'í', 'i' }, { 'ó', 'o' }, { 'ú', 'u' },{ 'Á', 'A' }, { 'É', 'E' }, { 'Í', 'I' }, { 'Ó', 'O' }, { 'Ú', 'U' }
        };

        var sb = new StringBuilder();
        foreach (char c in texto)
        {

            if (acentos.ContainsKey(c))
            {
                sb.Append(acentos[c]);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

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
