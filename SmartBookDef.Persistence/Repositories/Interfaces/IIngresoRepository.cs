using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;

namespace SmartBookDef.Persistence.Repositories.Interfaces;
public interface IIngresoRepository
{
    public bool ValidarCreacionIngreso(string libro, string lote);
    public void Crear(IngresoLibro ingresoLibro);
    public ConsultarIngresoResponse? Consultar(string id);
    public IEnumerable<ConsultarIngresoResponse> Consultar();
    public bool ExisteLibro(string idLibro);
    public bool SumarStock(string libro, int unidades);
    string? ObtenerLotePorId(string id);
}

