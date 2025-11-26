using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBookDef.Application.Services;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Enums;
using SmartBookDef.Domain.Exceptions;

namespace SmartBookDef.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;

    }

    
    [Authorize]
    [HttpPost]
    public ActionResult Crear(CrearUsuarioRequest request)
    {
        try
        {
            var usuario = _usuarioService.Crear(request);

            if (usuario is null)
                return BadRequest();

            return Created(string.Empty, usuario);
        }
        catch (BussinesRuleException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception exg)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, exg.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("verificar")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult VerificarCuenta(string token)
    {
        try
        {
            var resultado = _usuarioService.VerificarCuenta(token);

            if (!resultado)
                return BadRequest("El enlace es inválido o ha expirado.");

            return Ok("Cuenta verificada correctamente.");
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


    [AllowAnonymous]
    [HttpGet]
    public ActionResult Consultar(string? nombres, UsuarioRol? rol)
    {
        try
        {
            return Ok(_usuarioService.Consultar(nombres, rol));
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

    [Authorize]
    [HttpPut("{id}")]
    public ActionResult Actualizar(string id, ActualizarUsuarioRequest request)
    {
        try
        {
            var actualizado = _usuarioService.Actualizar(id, request);

            if (!actualizado)
                return NotFound("No existe un usuario con ese id");

            return NoContent();
        }
        catch (BussinesRuleException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

}
