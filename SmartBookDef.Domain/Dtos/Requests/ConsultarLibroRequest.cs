using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Requests;
public record ConsultarLibroRequest
(
   string? Nombre,
   string? Nivel,
   TipoLibro? Tipo,
   string? Editorial,
   string? Edicion
    );