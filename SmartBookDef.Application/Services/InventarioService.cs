using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Application.Services;
public class InventarioService
{
    private readonly IInventarioRepository _inventarioRepository;

    public InventarioService(IInventarioRepository inventarioRepository)
    {
        _inventarioRepository = inventarioRepository;
    }
    public IEnumerable<ConsultarInventarioResponse> ConsultarInventarioPorLote(string lote)
    {
        return _inventarioRepository.ConsultarInventarioPorLote(lote);
    }
}
