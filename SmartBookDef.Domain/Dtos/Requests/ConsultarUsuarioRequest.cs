using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Requests;
public record ConsultarUsuarioRequest
(
    string? Nombres,
    UsuarioRol? Rol
);

