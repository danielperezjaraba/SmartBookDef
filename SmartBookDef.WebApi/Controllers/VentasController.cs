using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartBookDef.Application.Services;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Exceptions;

namespace SmartBookDef.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VentasController : ControllerBase
{
    private readonly VentaService _ventaService;
    public VentasController(VentaService ventaService)
    {
        _ventaService = ventaService;

    }

    [HttpPost]
    public IActionResult CrearVenta([FromBody] CrearVentaRequest request)
    {
        try
        {

            var response = _ventaService.CrearVentaYEnviarPdf(request);

            return Ok(new
            {
                exito = true,
                mensaje = response.Mensaje,
                data = response
            });
        }
        catch (BussinesRuleException ex)
        {
            return BadRequest(new
            {
                exito = false,
                mensaje = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                exito = false,
                mensaje = "Ocurrió un error inesperado al procesar la venta",
                detalle = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public IActionResult ConsultarVenta(string id)
    {
        try
        {
            var venta = _ventaService.Consultar(id);

            if (venta == null)
            {
                return NotFound(new { mensaje = "Venta no encontrada" });
            }

            return Ok(new { exito = true, data = venta });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                exito = false,
                mensaje = "Error al consultar la venta",
                detalle = ex.Message
            });
        }
    }
    [HttpGet]
    public ActionResult Consultar([FromQuery] ConsultarVentaRequest request)
    {
        try
        {
            return Ok(_ventaService.Consultar(request));
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
