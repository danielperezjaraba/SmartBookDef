using SmartBookDef.Domain.DataAnotations;
using SmartBookDef.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.Dtos.Requests;
public record CrearUsuarioRequest
(
    [SoloNumeros]
    string Identificacion,
    string Contraseña,
    string Nombres,
    [CorreoCecar]
    string Correo,
    int Rol
);
