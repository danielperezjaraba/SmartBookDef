namespace SmartBookDef.Domain.Dtos.Responses;
public record ConsultarIngresoResponse
(
   string Id,
   DateOnly Fecha,
   string Libro,
   string Lote,
   int Unidades,
   decimal ValorCompra,
   decimal ValorVentaPublico
);