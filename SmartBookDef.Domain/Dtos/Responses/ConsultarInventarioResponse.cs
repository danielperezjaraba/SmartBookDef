namespace SmartBookDef.Domain.Dtos.Responses;
public record ConsultarInventarioResponse(
string Libro,
string Tipo,
int CantidadComprada,
int CantidadVendida,
int StockActual
);
