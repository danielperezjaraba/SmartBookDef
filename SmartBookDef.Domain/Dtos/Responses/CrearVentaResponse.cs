namespace SmartBookDef.Domain.Dtos.Responses;
public record CrearVentaResponse(
string Id,
string NumeroReciboPago,
DateOnly Fecha,
string LibroLote,
int Unidades,
string Cliente,
string Usuario,
string Observaciones,
string Mensaje = "Venta creada exitosamente"
);
