using SmartBookDef.Domain.DataAnotations;
using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Requests;
public record ActualizarUsuarioRequest
(
    string? Nombres,
    [CorreoCecar]
    string? Correo,
    UsuarioRol? Rol
);

