namespace SmartBookDef.Domain.Dtos.Requests;
public record ConsultarClienteRequest(
        string? id,
        string? identificacion,
        string? nombreCompleto,
        string? email,
        string? celular
        );
