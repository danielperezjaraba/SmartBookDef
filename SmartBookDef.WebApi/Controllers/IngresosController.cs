using Microsoft.AspNetCore.Mvc;
using SmartBookDef.Application.Services;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Exceptions;

namespace SmartBookDef.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class IngresosController : ControllerBase
{

    private readonly IngresoService _ingresoService;

    public IngresosController(IngresoService ingresoService)
    {
        _ingresoService = ingresoService;

    }

    [HttpPost]
    public ActionResult Crear(CrearIngresoRequest request)
    {
        try
        {
            var ingresoingreso = _ingresoService.Crear(request);

            if (ingresoingreso is null)
            {
                return BadRequest();
            }
            return Created(string.Empty, ingresoingreso);
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

    [HttpGet("{id}")]
    public ActionResult Consultar(string id)
    {
        try
        {
            var curso = _ingresoService.Consultar(id);
            if (curso is null)
            {
                return NotFound("No se encontraron datos para ese curso");
            }
            return Ok(curso);
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
    public ActionResult Consultar([FromQuery] ConsultarIngresoRequest request)
    {
        try
        {
            return Ok(_ingresoService.Consultar(request));
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

