using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Enums;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Persistence.Repositories;
public class UsuarioRepository : IUsuarioInterface
{
    private readonly string _connectionString;

    public UsuarioRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("smart_book");
    }


    private string Sql { get; set; }

    public void Crear(Usuario usuario)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            Sql = @"
            INSERT INTO usuarios (
                id,
                identificacion,
                contraseña,
                nombres,
                correo,
                rol,
                email_verificado,
                token_verificacion,
                token_expiracion
            ) VALUES (
                @id,
                @identidicacion,
                @contraseña,
                @nombres,
                @correo,
                @rol,
                @verificado,
                @token,
                @tokenExpiracion
            )";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@id", usuario.Id);
                cmd.Parameters.AddWithValue("@identidicacion", usuario.Identificacion);
                cmd.Parameters.AddWithValue("@contraseña", usuario.Contraseña);
                cmd.Parameters.AddWithValue("@nombres", usuario.Nombres);
                cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@rol", usuario.Rol);
                cmd.Parameters.AddWithValue("@verificado", usuario.EmailVerificado);
                cmd.Parameters.AddWithValue("@token", usuario.TokenVerificacion);
                cmd.Parameters.AddWithValue("@tokenExpiracion", usuario.TokenExpiracion.HasValue ? (object)usuario.TokenExpiracion.Value : DBNull.Value);


                cmd.ExecuteNonQuery();
            }
        }
    }


    public IEnumerable<Usuario> Consultar()
    {
        var usuarios = new List<Usuario>();


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = "SELECT * FROM usuarios";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                var reader = cmd.ExecuteReader();
                Sql = string.Empty;

                while (reader.Read())
                {
                    var id = reader["id"].ToString();
                    var identificacion = reader["identificacion"].ToString();
                    var contraseña = reader["contraseña"].ToString();
                    var nombres = reader["nombres"].ToString();
                    var correo = reader["correo"].ToString();
                    var rol = (UsuarioRol)Convert.ToInt32(reader["rol"]);


                    var usuario = new Usuario
                    {

                        Id = id,
                        Identificacion = identificacion,
                        Contraseña = contraseña,
                        Nombres = nombres,
                        Correo = correo,
                        Rol = rol

                    };

                    usuarios.Add(usuario);

                }
                return usuarios;

            }



        }
    }

    public bool Actualizar(string identificacion, ActualizarUsuarioRequest request)
    {


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"
                    update usuarios set 
                    nombres = @nombres, 
                    correo = @correo, 
                    rol = @rol 
                    where 
                    identificacion = @identificacion
                  ";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@nombres", request.Nombres);
                cmd.Parameters.AddWithValue("@correo", request.Correo);
                cmd.Parameters.AddWithValue("@rol", request.Rol);
                cmd.Parameters.AddWithValue("@identificacion", identificacion);

                return cmd.ExecuteNonQuery() > 0;

            }

        }

    }

    public bool ValidarCreacionIdentificacion(string identificacion)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"SELECT COUNT(id) FROM usuarios
                    WHERE identificacion = @identificacion";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@identificacion", identificacion);

                return (long)cmd.ExecuteScalar() == 0;

            }
        }
        return false;
    }

    public bool ValidarCreacionCorreo(string correo)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"SELECT COUNT(id) FROM usuarios
                    WHERE correo = @correo";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@correo", correo);

                return (long)cmd.ExecuteScalar() == 0;

            }
        }
        return false;
    }

    public Usuario? BuscarPorIdentificacion(string identificacion)
    {
        using var conexion = new MySqlConnection(_connectionString);
        conexion.Open();

        string sql = "SELECT * FROM usuarios WHERE identificacion = @identificacion";
        using var cmd = new MySqlCommand(sql, conexion);
        cmd.Parameters.AddWithValue("@identificacion", identificacion);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        DateTime? tokenExp = null;
        if (reader["token_expiracion"] != DBNull.Value)
            tokenExp = Convert.ToDateTime(reader["token_expiracion"]);

        DateTime? codigoExp = null;
        if (reader["codigo_expiracion"] != DBNull.Value)
            codigoExp = Convert.ToDateTime(reader["codigo_expiracion"]);

        return new Usuario
        {
            Id = reader["id"].ToString(),
            Identificacion = reader["identificacion"].ToString(),
            Contraseña = reader["contraseña"].ToString(),
            Nombres = reader["nombres"].ToString(),
            Correo = reader["correo"].ToString(),
            Rol = (UsuarioRol)Convert.ToInt32(reader["rol"]),
            EmailVerificado = Convert.ToBoolean(reader["email_verificado"]),
            TokenVerificacion = reader["token_verificacion"]?.ToString(),
            TokenExpiracion = tokenExp,
            CodigoRecuperacion = reader["codigo_recuperacion"]?.ToString(),
            CodigoExpiracion = codigoExp
        };
    }


    public Usuario? ObtenerPorToken(string token)
    {
        using var con = new MySqlConnection(_connectionString);
        con.Open();

        var sql = "SELECT * FROM usuarios WHERE token_verificacion = @token LIMIT 1";
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@token", token);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        DateTime? tokenExp = null;
        if (reader["token_expiracion"] != DBNull.Value)
        {
            tokenExp = Convert.ToDateTime(reader["token_expiracion"]);
        }

        return new Usuario
        {
            Id = reader["id"].ToString(),
            Identificacion = reader["identificacion"].ToString(),
            Contraseña = reader["contraseña"].ToString(),
            Nombres = reader["nombres"].ToString(),
            Correo = reader["correo"].ToString(),
            Rol = Enum.Parse<UsuarioRol>(reader["rol"].ToString()),
            EmailVerificado = Convert.ToBoolean(reader["email_verificado"]),
            TokenVerificacion = reader["token_verificacion"]?.ToString(),
            TokenExpiracion = tokenExp
        };
    }



    public bool MarcarVerificado(string id)
    {
        using var con = new MySqlConnection(_connectionString);
        con.Open();

        var sql = @"
        UPDATE usuarios
        SET email_verificado = 1,
            token_verificacion = NULL,
            token_expiracion = NULL
        WHERE id = @id";

        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }


    public bool GuardarCodigoRecuperacion(string id, string codigo, DateTime expiracion)
    {
        using var con = new MySqlConnection(_connectionString);
        con.Open();

        var sql = @"
        UPDATE usuarios
        SET codigo_recuperacion = @codigo,
            codigo_expiracion = @expiracion
        WHERE id = @id";

        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@codigo", codigo);
        cmd.Parameters.AddWithValue("@expiracion", expiracion);
        cmd.Parameters.AddWithValue("@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool ActualizarContrasena(string id, string nuevaHash)
    {
        using var con = new MySqlConnection(_connectionString);
        con.Open();

        var sql = @"
        UPDATE usuarios SET
            contraseña = @pass,
            codigo_recuperacion = NULL,
            codigo_expiracion = NULL
        WHERE id = @id";

        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@pass", nuevaHash);
        cmd.Parameters.AddWithValue("@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }
}

