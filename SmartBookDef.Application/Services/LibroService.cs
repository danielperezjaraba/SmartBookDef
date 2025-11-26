using SmartBookDef.Application.Extensions;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Enums;
using SmartBookDef.Domain.Exceptions;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Application.Services;
public class LibroService
{

    private readonly ILibroRepository _libroRepository;

    public LibroService(ILibroRepository libroRepository)
    {
        _libroRepository = libroRepository;
    }

    public CrearLibroResponse? Crear(CrearLibroRequest request)
    {
        int mes = DateTime.Now.Month;
        int semestre;
        if (mes <= 6)
        {
            semestre = 1;
        }
        else
        {
            semestre = 2;
        }

        string lote = $"{DateTime.Now.Year}-{semestre}";

        var valido = _libroRepository.ValidarCreacion(request.Nombre, request.Nivel, (TipoLibro)request.Tipo, request.Edicion, lote);


        if (!valido)
        {
            throw new BussinesRuleException("Ya existe un libro con esas caracteristicas");
        }

        var libro = new Libro
        {
            Id = DateTime.Now.Ticks.ToString(),
            Nombre = request.Nombre.Sanitize().RemoveAccents(),
            Nivel = request.Nivel.Sanitize(),
            Stock = request.Stock,
            Tipo = request.Tipo.ToEnum<TipoLibro>(),
            Editorial = request.Editorial.Sanitize(),
            Edicion = request.Edicion.Sanitize(),
            Lote = lote
        };

        _libroRepository.Crear(libro);
        return new CrearLibroResponse(libro.Id, libro.Nombre, libro.Tipo, lote);
    }

    public IEnumerable<ConsultarLibroResponse> Consultar(ConsultarLibroRequest request)
    {
        var libros = _libroRepository.Consultar().AsEnumerable();

        if (request.Nombre is not null)
            libros = libros.Where(l => l.Nombre == request.Nombre);

        if (request.Nivel is not null)
            libros = libros.Where(l => l.Nivel == request.Nivel);

        if (request.Tipo is not null)
            libros = libros.Where(l => l.Tipo == request.Tipo);

        if (request.Editorial is not null)
            libros = libros.Where(l => l.Editorial == request.Editorial);

        if (request.Edicion is not null)
            libros = libros.Where(l => l.Edicion == request.Edicion);

        return libros.Select(l => new ConsultarLibroResponse(
            l.Id,
            l.Nombre,
            l.Nivel,
            l.Stock,
            l.Tipo,
            l.Editorial,
            l.Edicion
        )); ;
    }

    public ConsultarLibroResponse Consultar(string id)
    {
        return _libroRepository.Consultar(id);
    }

    public bool Actualizar(string id, ActualizarLibroRequest request)
    {

        return _libroRepository.Actualizar(id, request);

    }

}

