using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.Entities;
public class IngresoLibro
{
    [Required]
    public string Id { get; init; }
    [Required]
    public DateOnly Fecha { get; init; }
    [Required]
    public string Libro { get; set; }
    [Required]
    public string Lote { get; init; }
    [Required]
    public int Unidades { get; set; }
    [Required]
    public decimal ValorCompra { get; set; }
    [Required]
    public decimal ValorVentaPublico { get; set; }
}

