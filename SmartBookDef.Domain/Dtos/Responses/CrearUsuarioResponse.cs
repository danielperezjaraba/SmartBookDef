using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Responses;
public record CrearUsuarioResponse
(
   string Id,
   string Identificacion,
   UsuarioRol Rol
);
