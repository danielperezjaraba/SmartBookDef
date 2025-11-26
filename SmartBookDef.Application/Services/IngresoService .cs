using SmartBookDef.Application.Extensions;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Exceptions;
using SmartBookDef.Persistence.Repositories;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Application.Services;
public class IngresoService
{

    private readonly IIngresoRepository _ingresoRepository;

    public IngresoService(IIngresoRepository ingresoRepository)
    {
        _ingresoRepository = ingresoRepository;
    }

    public CrearIngresoResponse? Crear(CrearIngresoRequest request)
    {
        var libroExiste = _ingresoRepository.ExisteLibro(request.Libro);


        if (!libroExiste)
        {
            throw new BussinesRuleException("El libro que intenta ingresar no existe en la base de datos.");
        }

        var loteDelLibro = _ingresoRepository.ObtenerLotePorId(request.Libro);

        if (loteDelLibro is null)
        {
            throw new BussinesRuleException("El libro no tiene lote asociado.");
        }

        string lote = loteDelLibro;

        var valido = _ingresoRepository.ValidarCreacionIngreso(request.Libro, lote);


        if (!valido)
        {
            throw new BussinesRuleException("Ya existe un codigo de lote con esas caracteristicas");
        }


        var ingresoLibro = new IngresoLibro
        {
            Id = DateTime.Now.Ticks.ToString(),
            Fecha = DateOnly.FromDateTime(DateTime.Now),
            Libro = request.Libro.Sanitize().RemoveAccents(),
            Lote = lote,
            Unidades = request.Unidades,
            ValorCompra = request.ValorCompra,
            ValorVentaPublico = request.ValorVentaPublico

        };

        _ingresoRepository.Crear(ingresoLibro);

        var actualizado = _ingresoRepository.SumarStock(request.Libro, request.Unidades);

        if (!actualizado)
            throw new BussinesRuleException("No se pudo actualizar el stock del libro.");

        return new CrearIngresoResponse(ingresoLibro.Id, ingresoLibro.Fecha, ingresoLibro.Libro, ingresoLibro.Lote, ingresoLibro.Unidades, ingresoLibro.ValorCompra, ingresoLibro.ValorVentaPublico);
    }

    public ConsultarIngresoResponse? Consultar(string id)
    {
        return _ingresoRepository.Consultar(id);
    }

    public IEnumerable<ConsultarIngresoResponse> Consultar(ConsultarIngresoRequest request)
    {
        var ingresos = _ingresoRepository.Consultar().AsEnumerable();

        if (request.Desde is not null)
            ingresos = ingresos.Where(i => i.Fecha >= request.Desde);

        if (request.Hasta is not null)
            ingresos = ingresos.Where(i => i.Fecha <= request.Hasta);

        if (request.Lote is not null)
            ingresos = ingresos.Where(i => i.Lote == request.Lote);

        if (request.Libro is not null)
            ingresos = ingresos.Where(i => i.Libro == request.Libro);

        return ingresos.Select(i => new ConsultarIngresoResponse(
            i.Id,
            i.Fecha,
            i.Libro,
            i.Lote,
            i.Unidades,
            i.ValorCompra,
            i.ValorVentaPublico
        ));
    }

}

