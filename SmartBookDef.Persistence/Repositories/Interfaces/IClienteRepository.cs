using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;

namespace SmartBookDef.Persistence.Repositories.Interfaces;
public interface IClienteRepository
{
    public bool ValidarCreacionCliente(string identificacion);
    public void Crear(Cliente cliente);
    public ConsultarClienteResponse? Consultar(string identificacion);
    public bool Actualizar(string identificacion, ActualizarClienteRequest request);
    public IEnumerable<ConsultarClienteResponse> Consultar();

}
