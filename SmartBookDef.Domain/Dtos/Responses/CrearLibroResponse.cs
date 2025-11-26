using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Responses;
public record CrearLibroResponse
(
    string Id,
    string Nombre,
    TipoLibro Tipo,
    string Lote
);
