using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Entities;

namespace SmartBookDef.Persistence.Repositories.Interfaces;
public interface IUsuarioInterface
{
    public void Crear(Usuario usuario);
    public IEnumerable<Usuario> Consultar();
    public bool Actualizar(string identificacion, ActualizarUsuarioRequest actualizarUsuario);
    public bool ValidarCreacionIdentificacion(string identificacion);
    public bool ValidarCreacionCorreo(string correo);
    public Usuario? BuscarPorIdentificacion(string identificacion);
    public Usuario? ObtenerPorToken(string token);
    public bool MarcarVerificado(string id);
    public bool GuardarCodigoRecuperacion(string id, string codigo, DateTime expiracion);
    public bool ActualizarContrasena(string id, string nuevaHash);

}
