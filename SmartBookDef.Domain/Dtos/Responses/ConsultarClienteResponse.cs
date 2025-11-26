namespace SmartBookDef.Domain.Dtos.Responses;
public record ConsultarClienteResponse(
    string id,
    string identificacion,
    string nombreCompleto,
    string email,
    string celular
    );
