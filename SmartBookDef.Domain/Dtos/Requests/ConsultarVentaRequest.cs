namespace SmartBookDef.Domain.Dtos.Requests;
public record ConsultarVentaRequest(

    DateOnly? Desde,
    DateOnly? Hasta,
    string? Cliente,
    string? LibroLote

    );
