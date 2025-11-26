namespace SmartBookDef.Domain.Dtos.Requests;
public record CrearVentaRequest(
List<VentaLibroItem> Libros,
string Cliente,
string Usuario,
string Observaciones
);

public record VentaLibroItem(
    string LibroLote,
    int Unidades
);