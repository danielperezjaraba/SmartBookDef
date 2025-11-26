using SmartBookDef.Domain.Dtos.Responses;

namespace SmartBookDef.Persistence.Repositories.Interfaces;
public interface IInventarioRepository
{
    IEnumerable<ConsultarInventarioResponse> ConsultarInventarioPorLote(string lote);
}
