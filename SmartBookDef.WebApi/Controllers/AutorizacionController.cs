using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBookDef.Application.Services;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Exceptions;

namespace SmartBookDef.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AutorizacionController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public AutorizacionController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult InicioSesion(LoginRequest request)
    {
        try
        {
            var usuario = _usuarioService.Autenticar(request.Identificacion, request.Contrasena);

            if (usuario == null)
            {
                return Unauthorized("Identificación o contraseña incorrectas.");
            }

            if ((int)usuario.Rol != 1)
            {
                return Unauthorized("No tiene permisos para iniciar sesión.");
            }

            var token = _usuarioService.GenerarJwt(usuario);

            return Ok(new { token });

        }
        catch (BussinesRuleException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("enviar-codigo")]
    public IActionResult EnviarCodigo(RecuperarContrasenaRequest request)
    {
        var ok = _usuarioService.EnviarCodigoRecuperacion(request.Identificacion, request.Contrasena);

        if (!ok)
            return BadRequest("Identificación o contraseña incorrecta.");

        return Ok("Código enviado al correo.");
    }

    [AllowAnonymous]
    [HttpPost("restablecer")]
    public IActionResult Restablecer(RestablecerContrasenaRequest request)
    {
        try
        {
            var ok = _usuarioService.RestablecerContrasena(
                request.Identificacion,
                request.Codigo,
                request.NuevaContrasena
            );

            if (!ok)
                return BadRequest("Datos inválidos.");

            return Ok("Contraseña actualizada correctamente.");
        }
        catch (BussinesRuleException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}
