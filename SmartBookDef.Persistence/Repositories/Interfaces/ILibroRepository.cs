using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Persistence.Repositories.Interfaces;
public interface ILibroRepository
{
    void Crear(Libro libro);
    ConsultarLibroResponse? Consultar(string id);
    IEnumerable<ConsultarLibroResponse> Consultar();
    bool Actualizar(string id, ActualizarLibroRequest request);
    public bool ValidarCreacion(string nombre, string nivel, TipoLibro tipo, string edicion, string lote);

}