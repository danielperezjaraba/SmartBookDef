using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Responses;
public record ConsultarUsuarioResponse
(
string Id,
string Identificacion,
string Contraseña,
string Nombres,
string Correo,
UsuarioRol Rol
);
