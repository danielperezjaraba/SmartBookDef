namespace SmartBookDef.Domain.Dtos.Requests;
public record CrearIngresoRequest(
        string Libro,
        int Unidades,
        decimal ValorCompra,
        decimal ValorVentaPublico
        );

