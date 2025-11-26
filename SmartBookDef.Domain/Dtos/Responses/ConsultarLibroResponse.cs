using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Responses;
public record ConsultarLibroResponse
(
   string Id,
   string Nombre,
   string Nivel,
   int Stock,
   TipoLibro Tipo,
   string Editorial,
   string Edicion
);