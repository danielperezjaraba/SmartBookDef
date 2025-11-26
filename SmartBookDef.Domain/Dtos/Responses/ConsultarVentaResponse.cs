namespace SmartBookDef.Domain.Dtos.Responses;
public record ConsultarVentaResponse(
    string Id,
    string NumeroReciboPago,
    DateOnly Fecha,
    string LibroLote,
    int Unidades,
    string Cliente,
    string Usuario,
    string Observaciones,
    string CorreoCliente,
    decimal PrecioUnitario,
    decimal Total
    );
