using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartBookDef.Application.Extensions;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Enums;
using SmartBookDef.Domain.Exceptions;
using SmartBookDef.Persistence.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace SmartBookDef.Application.Services;
public class UsuarioService
{
    private readonly EmailService _emailService;
    private readonly IUsuarioInterface _usuarioRepository;
    private readonly IConfiguration _configuration;
    public UsuarioService(IUsuarioInterface usuarioRepository, EmailService emailService, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
        _configuration = configuration;
    }



    public CrearUsuarioResponse? Crear(CrearUsuarioRequest request)
    {
        var validoCorreo = _usuarioRepository.ValidarCreacionCorreo(request.Correo);
        var validoIdentificacion = _usuarioRepository.ValidarCreacionIdentificacion(request.Identificacion);
        if (!validoCorreo)
        {
            throw new BussinesRuleException("Ya existe un usuario con ese Correo");
        }
    
        if (!validoIdentificacion)
        {
            throw new BussinesRuleException("Ya existe un usuario con esa Identificacion o");
        }


        if (request.Contraseña.Length < 8)
            throw new BussinesRuleException("La contraseña debe tener mínimo 8 caracteres.");

        var token = Guid.NewGuid().ToString("N");
        var expiracion = DateTime.UtcNow.AddHours(1); 

        var usuario = new Usuario
        {
            Id = DateTime.Now.Ticks.ToString(),
            Identificacion = request.Identificacion.Sanitize(),
            Contraseña = PasswordHasher.Hash(request.Contraseña),
            Nombres = request.Nombres.Sanitize().RemoveAccents(),
            Correo = request.Correo,
            Rol = request.Rol.ToEnum<UsuarioRol>(),
            EmailVerificado = false,
            TokenVerificacion = token,
            TokenExpiracion = expiracion
        };

        _usuarioRepository.Crear(usuario);

        string url = $"http://localhost:5147/api/usuarios/verificar?token={token}";
        string mensaje = $"Haz clic en el siguiente enlace para verificar tu cuenta:<br><a href='{url}'>{url}</a>";
        _emailService.Enviar(usuario.Correo, "Verificación de cuenta", mensaje);


        return new CrearUsuarioResponse(usuario.Id, usuario.Identificacion, usuario.Rol);
    }




    public IEnumerable<ConsultarUsuarioResponse> Consultar(string? nombres, UsuarioRol? rol)
    {
        var usuarios = _usuarioRepository.Consultar().AsEnumerable();

        if (nombres is not null)
            usuarios = usuarios.Where(l => l.Nombres == nombres);

        if (rol is not null)
            usuarios = usuarios.Where(l => l.Rol == rol);


        return usuarios.Select(l => new ConsultarUsuarioResponse(
            l.Id,
            l.Identificacion,
            l.Contraseña,
            l.Nombres,
            l.Correo,
            l.Rol
        ));
    }


    public bool Actualizar(string identificacion, ActualizarUsuarioRequest request)
    {

        var usuarioOriginal = _usuarioRepository.BuscarPorIdentificacion(identificacion);

        if (usuarioOriginal is null)
        {
            return false;
        }

        return _usuarioRepository.Actualizar(identificacion, request);
    }

    public bool VerificarCuenta(string token)
    {
        var usuario = _usuarioRepository.ObtenerPorToken(token);
        if (usuario == null) return false;

        if (usuario.TokenExpiracion.HasValue && usuario.TokenExpiracion.Value < DateTime.UtcNow)
            return false;

        var marcado = _usuarioRepository.MarcarVerificado(usuario.Id);
        if (!marcado) return false;

        _emailService.Enviar(
            usuario.Correo,
            "Cuenta verificada",
            $"<h3>Hola {usuario.Nombres},</h3><p>Tu cuenta fue verificada correctamente.</p>"
        );

        return true;
    }

    public Usuario? Autenticar(string identificacion, string contrasena)
    {
        var usuario = _usuarioRepository.BuscarPorIdentificacion(identificacion);

        if (usuario == null)
            return null;

        if (!PasswordHasher.Verify(contrasena, usuario.Contraseña))
            return null;

        if (!usuario.EmailVerificado)
        {
            throw new BussinesRuleException("Debe verificar su correo antes de iniciar sesión.");
        }

        return usuario;
    }

    public string GenerarJwt(Usuario u)
    {
        var jwt = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));

        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("id", u.Id),
            new Claim("rol", u.Rol.ToString()),
            new Claim("email", u.Correo)
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresInMinutes"])),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool EnviarCodigoRecuperacion(string identificacion, string contrasena)
    {
        var usuario = _usuarioRepository.BuscarPorIdentificacion(identificacion);

        if (usuario == null)
            return false;

        if (!PasswordHasher.Verify(contrasena, usuario.Contraseña))
            return false;

        if (!usuario.EmailVerificado)
            throw new BussinesRuleException("Debe verificar su correo antes de recuperar la contraseña.");

        string codigo = Guid.NewGuid().ToString("N")[..8].ToUpper();

        DateTime expiracion = DateTime.UtcNow.AddMinutes(5);

        _usuarioRepository.GuardarCodigoRecuperacion(usuario.Id, codigo, expiracion);

        _emailService.Enviar(
            usuario.Correo,
            "Recuperación de contraseña",
            $"Tu código de recuperación es: <h2>{codigo}</h2><p>Tiene una validez de <b>5 minutos</b>.</p>"
        );

        return true;
    }

    public bool RestablecerContrasena(string identificacion, string codigo, string nuevaContrasena)
    {
        var usuario = _usuarioRepository.BuscarPorIdentificacion(identificacion);

        if (usuario == null)
            return false;

        if (usuario.CodigoRecuperacion != codigo)
            return false;

        if (usuario.CodigoExpiracion == null || usuario.CodigoExpiracion < DateTime.UtcNow)
            throw new BussinesRuleException("El código de recuperación ha expirado.");

        if (nuevaContrasena.Length < 8)
            throw new BussinesRuleException("La contraseña debe tener mínimo 8 caracteres.");

        var nuevaHash = PasswordHasher.Hash(nuevaContrasena);

        return _usuarioRepository.ActualizarContrasena(usuario.Id, nuevaHash);
    }

}

