using Microsoft.AspNetCore.Mvc;
using SmartBookDef.Application.Services;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Exceptions;

namespace SmartBookDef.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LibrosController : ControllerBase
{

    private readonly LibroService _libroService;

    public LibrosController(LibroService libroService)
    {
        _libroService = libroService;

    }

    [HttpPost]
    public ActionResult Crear(CrearLibroRequest request)
    {
        try
        {
            var libro = _libroService.Crear(request);

            if (libro is null)
            {
                return BadRequest();
            }
            return Created(string.Empty, libro);
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
            var curso = _libroService.Consultar(id);
            if (curso is null)
            {
                return NotFound();
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
    public ActionResult Consultar([FromQuery] ConsultarLibroRequest request)
    {
        try
        {
            return Ok(_libroService.Consultar(request));
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

    [HttpPut("{id}")]
    public ActionResult Actualizar(string id, ActualizarLibroRequest request)
    {

        try
        {
            var libro = _libroService.Actualizar(id, request);

            if (!libro)
            {
                return NotFound();
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
