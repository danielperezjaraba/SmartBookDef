namespace SmartBookDef.Domain.Dtos.Requests;
public record ConsultarIngresoRequest
(
    DateOnly? Desde,
    DateOnly? Hasta,
    string? Lote,
    string? Libro
    );
