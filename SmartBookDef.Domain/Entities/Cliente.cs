using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.Entities;
public class Cliente
{
    [Required]
    public string Id { get; init; }
    [Required]
    public string Identificacion { get; set; }
    [Required]
    public string NombreCompleto { get; init; }
    [Required]
    public string EmailUsuario { get; set; }
    [Required]
    public string Celular { get; set; }
    [Required]
    public DateOnly FechaNacimiento { get; set; }

}
