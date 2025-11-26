namespace SmartBookDef.Application.Options;
public record Correo
{
    public string cuenta { get; set; } 
    public string contrasenia { get; set; }
    public int host { get; set; }

}
