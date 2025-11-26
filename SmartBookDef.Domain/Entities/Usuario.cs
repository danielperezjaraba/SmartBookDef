using SmartBookDef.Domain.DataAnotations;
using SmartBookDef.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.Entities;
public class Usuario
{
    [Required]
   
    public string Id { get; set; }

    [Required]
    public string Identificacion { get; init; }

    [Required]
    public string Contraseña { get; init; }

    [Required]
    public string Nombres { get; set; }

    [Required]
    [CorreoCecar]
    public string Correo { get; set; }

    [Required]
    public UsuarioRol Rol { get; set; }

    public bool EmailVerificado { get; set; }
    public string TokenVerificacion { get; set; }

    public DateTime? TokenExpiracion { get; set; }

    public string? CodigoRecuperacion { get; set; }
    public DateTime? CodigoExpiracion { get; set; }
}