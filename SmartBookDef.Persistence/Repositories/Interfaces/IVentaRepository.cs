using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;

namespace SmartBookDef.Persistence.Repositories.Interfaces;
public interface IVentaRepository
{
    public void Crear(VentaLibro venta);
    public bool DisminuirStock(string libro, int unidades);
    public bool StockDisponible(string libroLote, int unidadesSolicitadas);
    public string NumeroRecibo();
    public string Usuario(string usuario);
    public string CorreoCliente(string identificacionCliente);
    public decimal? PrecioLibro(string libroLote);
    public ConsultarVentaResponse? Consultar(string id);
    public IEnumerable<ConsultarVentaResponse> Consultar();
}
