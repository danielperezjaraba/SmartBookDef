using Microsoft.AspNetCore.Mvc;
using SmartBookDef.Application.Services;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Exceptions;

namespace SmartBookDef.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{

    private readonly CLienteService _clienteService;
    public ClientesController(CLienteService clienteService)
    {
        _clienteService = clienteService;

    }

    [HttpPost]
    public ActionResult Crear(CrearClienteRequest request)
    {
        try
        {

            var cliente = _clienteService.Crear(request);

            if (cliente is null)
            {
                return BadRequest();
            }
            return Created(string.Empty, cliente);
        }
        catch (BussinesRuleException exb)
        {
            return UnprocessableEntity(exb.Message);
        }
        catch (Exception exg)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, exg.Message);
        }
    }


    [HttpGet("{identificacion}")]
    public ActionResult Consultar(string identificacion)
    {
        try
        {
            var cliente = _clienteService.Consultar(identificacion);
            if (cliente is null)
            {
                return NotFound("No se encontraron datos para ese cliente");
            }
            return Ok(cliente);
        }
                catch (BussinesRuleException exb)
        {
            return UnprocessableEntity(exb.Message);
        }
        catch (Exception exg)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, exg.Message);
        }
    }

    [HttpGet]
    public ActionResult Consultar([FromQuery] ConsultarClienteRequest request)
    {
        try
        {
            return Ok(_clienteService.Consultar(request));
        }
        catch (BussinesRuleException exb)
        {
            return UnprocessableEntity(exb.Message);
        }
        catch (Exception exg)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, exg.Message);
        }
    }

    [HttpPut("{identificacion}")]
    public ActionResult Actualizar(string identificacion, ActualizarClienteRequest request)
    {
        try
        {
            var cliente = _clienteService.Actualizar(identificacion, request);

            if (!cliente)
            {
                return NotFound("No se encontraron datos para ese cliente");
            }
            return NoContent();
        }
        catch (BussinesRuleException exb)
        {
            return UnprocessableEntity(exb.Message);
        }
        catch (Exception exg)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, exg.Message);
        }
    }
}
