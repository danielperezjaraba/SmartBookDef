using SmartBookDef.Application.Extensions;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Exceptions;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Application.Services;
public class CLienteService
{
    private readonly IClienteRepository _clienteRepository;

    public CLienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public ClienteResponse? Crear(CrearClienteRequest request)
    {
        var ClienteRepetido = _clienteRepository.ValidarCreacionCliente(request.identificacion);
        var EmailRepetido = _clienteRepository.ValidarCreacionCliente(request.emailUsuario);
        var CelularRepetido = _clienteRepository.ValidarCreacionCliente(request.celular);


        if (!ClienteRepetido)
        {
            throw new BussinesRuleException("Ya existe un cliente con esta identificacion");
        }
        else if (!EmailRepetido)
        {

            throw new BussinesRuleException("Ya existe un cliente con este Email");

        }
        else if (!CelularRepetido)
        {

            throw new BussinesRuleException("Ya existe un cliente con este numero de Celular");

        }

        int edad = request.fechaNacimiento.ToDateTime(TimeOnly.MinValue).CalcularEdad();

        if (edad < 14)
        {
            throw new BussinesRuleException("No se permite registrar usuarios menores de 14 años.");
        }

        var cliente = new Cliente
        {
            Id = DateTime.Now.Ticks.ToString(),
            Identificacion = request.identificacion,
            NombreCompleto = request.nombreCompleto.Sanitize().RemoveAccents(),
            EmailUsuario = request.emailUsuario,
            Celular = request.celular,
            FechaNacimiento = request.fechaNacimiento
        };



        _clienteRepository.Crear(cliente);

        return new ClienteResponse(cliente.Id, cliente.Identificacion, cliente.NombreCompleto, cliente.EmailUsuario, cliente.Celular, cliente.FechaNacimiento);

    }


    public ConsultarClienteResponse? Consultar(string identificacion)
    {
        return _clienteRepository.Consultar(identificacion);
    }

    public IEnumerable<ConsultarClienteResponse> Consultar(ConsultarClienteRequest request)
    {
        var clientes = _clienteRepository.Consultar().AsEnumerable();

        if (request.id is not null)
            clientes = clientes.Where(c => c.id == request.id);

        if (request.nombreCompleto is not null)
            clientes = clientes.Where(c => c.nombreCompleto == request.nombreCompleto);

        if (request.email is not null)
            clientes = clientes.Where(c => c.email == request.email);

        if (request.celular is not null)
            clientes = clientes.Where(c => c.celular == request.celular);

        return clientes.Select(c => new ConsultarClienteResponse(
            c.id,
            c.identificacion,
            c.nombreCompleto,
            c.email,
            c.celular
        ));
    }
    public bool Actualizar(string identificacion, ActualizarClienteRequest request)
    {
        int edad = request.fechaNacimiento.ToDateTime(TimeOnly.MinValue).CalcularEdad();

        if (edad < 14)
        {
            throw new BussinesRuleException("No se permite registrar usuarios menores de 14 años.");
        }
        return _clienteRepository.Actualizar(identificacion, request);

    }



}
