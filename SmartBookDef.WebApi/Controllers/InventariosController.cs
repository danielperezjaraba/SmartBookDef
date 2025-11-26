using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartBookDef.Application.Services;
using SmartBookDef.Domain.Exceptions;

namespace SmartBookDef.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class InventariosController : ControllerBase
{
    private readonly InventarioService _inventarioService;

    public InventariosController(InventarioService inventarioService)
    {
        _inventarioService = inventarioService;

    }
    [HttpGet("inventario/{lote}")]
    public ActionResult ConsultarInventarioPorLote(string lote)
    {
        try
        {
            var resultado = _inventarioService.ConsultarInventarioPorLote(lote);

            if (!resultado.Any())
                return NotFound("No se encontraron datos para ese lote");

            return Ok(resultado);
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
