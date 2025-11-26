using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Persistence.Repositories;
public class ClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public ClienteRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("smart_book");
    }

    private string Sql { get; set; }
    private const string FORMATO_FECHA = "yyyy-MM-dd";

    public bool ValidarCreacionCliente(string identificacion)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"SELECT COUNT(id) 
                    FROM clientes 
                    WHERE identificacion =@identificacion";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@identificacion", identificacion);

                return (long)cmd.ExecuteScalar() == 0;

            }
        }
    }
    public void Crear(Cliente cliente)
    {


        using (var conexion = new MySqlConnection(_connectionString))
        {


            conexion.Open();

            Sql = @"INSERT INTO clientes VALUES(@id,@identificacion,@nombreCompleto,@emailUsuario,@celular,@fechaNacimiento)";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {


                cmd.Parameters.AddWithValue("@id", cliente.Id);
                cmd.Parameters.AddWithValue("@identificacion", cliente.Identificacion);
                cmd.Parameters.AddWithValue("@nombreCompleto", cliente.NombreCompleto);
                cmd.Parameters.AddWithValue("@emailUsuario", cliente.EmailUsuario);
                cmd.Parameters.AddWithValue("@celular", cliente.Celular);
                cmd.Parameters.AddWithValue("@fechaNacimiento", cliente.FechaNacimiento.ToString(FORMATO_FECHA));

                if (cmd.ExecuteNonQuery() > 0)
                {


                }

            }

        }

    }
    public ConsultarClienteResponse? Consultar(string identificacion)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();
            Sql = @"SELECT id,
	                       identificacion,
                           nombre_completo,
                           email_usuario,
                           celular
                    FROM clientes WHERE identificacion = @identificacion";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@identificacion", identificacion);
                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {

                    return null;
                }

                reader.Read();

                var id = reader["id"].ToString();
                var nombre = reader["nombre_completo"].ToString();
                var email = reader["email_usuario"].ToString();
                var celular = reader["celular"].ToString();


                return new ConsultarClienteResponse(

                    id,
                    identificacion,
                    nombre,
                    email,
                    celular
                );

            }
        }
    }

    public bool Actualizar(string identificacion, ActualizarClienteRequest request)
    {


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"UPDATE clientes
                   SET nombre_completo = @nombreCompleto, fecha_nacimiento=@fechaNacimiento,
                    email_usuario = @emailUsuario, celular = @celular
                   WHERE identificacion =@identificacion";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@identificacion", identificacion);
                cmd.Parameters.AddWithValue("@nombreCompleto", request.nombreCompleto);
                cmd.Parameters.AddWithValue("@fechaNacimiento", request.fechaNacimiento.ToString(FORMATO_FECHA));
                cmd.Parameters.AddWithValue("@emailUsuario", request.emailUsuario);
                cmd.Parameters.AddWithValue("@celular", request.celular);

                return cmd.ExecuteNonQuery() > 0;


            }

        }

    }

    public IEnumerable<ConsultarClienteResponse> Consultar()
    {
        var clientes = new List<ConsultarClienteResponse>();


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = "SELECT * FROM clientes";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                var reader = cmd.ExecuteReader();
                Sql = string.Empty;

                while (reader.Read())
                {
                    var id = reader["id"].ToString();
                    var identificacion = reader["identificacion"].ToString();
                    var nombre = reader["nombre_completo"].ToString();
                    var email = reader["email_usuario"].ToString();
                    var celular = reader["celular"].ToString();

                    var cliente = new ConsultarClienteResponse
                    (

                        id,
                        identificacion,
                        nombre,
                        email,
                        celular

                    );

                    clientes.Add(cliente);

                }
                return clientes;

            }



        }
    }

}
