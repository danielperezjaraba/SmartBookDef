namespace SmartBookDef.Domain.Dtos.Requests;
public record RestablecerContrasenaRequest(
    string Identificacion,
    string Codigo,
    string NuevaContrasena
);