namespace SmartBookDef.Domain.Entities;
public class VentaLibro
{
    public string Id { get; set; }
    public string NumeroReciboPago { get; set; }
    public DateOnly Fecha { get; set; }
    public string LibroLote { get; set; }
    public int Unidades { get; set; }
    public string Cliente { get; set; }
    public string Usuario { get; set; }
    public string Observaciones { get; set; }
}
