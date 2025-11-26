using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Requests;
public record ActualizarLibroRequest
(
   string? Nombre,
   string? Nivel,
   TipoLibro? Tipo,
   string? Editorial,
   string? Edicion
    );
