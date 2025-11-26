namespace SmartBookDef.Domain.Dtos.Responses;
public record ClienteResponse(
    string Id,
    string Identificacion,
    string NombreCompleto,
    string EmailUsuario,
    string Celular,
    DateOnly FechaNacimiento
    );
