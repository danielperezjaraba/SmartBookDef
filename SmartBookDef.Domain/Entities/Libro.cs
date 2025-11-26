using SmartBookDef.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.Entities;
public class Libro
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Nivel { get; set; }
    [Required]
    public int Stock { get; init; }
    [Required]
    public TipoLibro Tipo { get; set; }

    [MaxLength(150)]
    public string Editorial { get; set; }
    [Required]
    public string Edicion { get; set; }
    [Required]
    [StringLength(6)] 
    public string Lote { get; set; }

}
