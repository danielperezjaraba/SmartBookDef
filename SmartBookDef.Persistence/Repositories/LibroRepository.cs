using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Enums;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Persistence.Repositories;
public class LibroRepository : ILibroRepository
{

    private readonly string _connectionString;

    public LibroRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("smart_book");
    }

    private string Sql { get; set; }

    public void Crear(Libro libro)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"
                    INSERT INTO Libros VALUES(
                    @id, 
                    @nombre, 
                    @nivel, 
                    @stock, 
                    @tipo, 
                    @editorial, 
                    @edicion,
                    @lote
                    )";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@id", libro.Id);
                cmd.Parameters.AddWithValue("@nombre", libro.Nombre);
                cmd.Parameters.AddWithValue("@nivel", libro.Nivel);
                cmd.Parameters.AddWithValue("@stock", libro.Stock);
                cmd.Parameters.AddWithValue("@tipo", libro.Tipo);
                cmd.Parameters.AddWithValue("@editorial", libro.Editorial);
                cmd.Parameters.AddWithValue("@edicion", libro.Edicion);
                cmd.Parameters.AddWithValue("@lote", libro.Lote);


                if (cmd.ExecuteNonQuery() > 0)
                {


                }

            }

        }

    }

    public ConsultarLibroResponse? Consultar(string id)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();
            Sql = @"SELECT * FROM libros where 
                    id = @id
                  ";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {

                    return null;
                }

                reader.Read();

                var nombre = reader["nombre"].ToString();
                var nivel = reader["nivel"].ToString();
                var stock = Convert.ToInt32(reader["stock"]);
                var tipo = (TipoLibro)Convert.ToInt32(reader["tipo"]);
                var editorial = reader["editorial"].ToString();
                var edicion = reader["edicion"].ToString();

                return new ConsultarLibroResponse(

                    id,
                    nombre,
                    nivel,
                    stock,
                    tipo,
                    editorial,
                    edicion
                );

            }

        }
    }

    public IEnumerable<ConsultarLibroResponse> Consultar()
    {
        var libros = new List<ConsultarLibroResponse>();


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = "SELECT * FROM libros";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                var reader = cmd.ExecuteReader();
                Sql = string.Empty;

                while (reader.Read())
                {
                    var id = reader["id"].ToString();
                    var nombre = reader["nombre"].ToString();
                    var nivel = reader["nivel"].ToString();
                    var stock = Convert.ToInt32(reader["stock"]);
                    var tipo = (TipoLibro)Convert.ToInt32(reader["tipo"]);
                    var editorial = reader["editorial"].ToString();
                    var edicion = reader["edicion"].ToString();


                    var libro = new ConsultarLibroResponse
                    (

                        id,
                        nombre,
                        nivel,
                        stock,
                        tipo,
                        editorial,
                        edicion

                    );

                    libros.Add(libro);

                }
                return libros;

            }



        }
    }

    public bool Actualizar(string id, ActualizarLibroRequest actualizarLibro)
    {


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"
                    update libros set 
                    nombre = @nombre, 
                    nivel = @nivel, 
                    tipo = @tipo, 
                    editorial = @editorial, 
                    edicion = @edicion 
                    where 
                    id = @id
                  ";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@nombre", actualizarLibro.Nombre);
                cmd.Parameters.AddWithValue("@nivel", actualizarLibro.Nivel);
                cmd.Parameters.AddWithValue("@tipo", actualizarLibro.Tipo);
                cmd.Parameters.AddWithValue("@editorial", actualizarLibro.Editorial);
                cmd.Parameters.AddWithValue("@edicion", actualizarLibro.Edicion);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;

            }

        }

    }


    public bool ValidarCreacion(string nombre, string nivel, TipoLibro tipo, string edicion, string lote)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"SELECT COUNT(id) FROM libros
                    WHERE nombre = @nombre AND nivel = @nivel AND tipo = @tipo AND edicion = @edicion AND lote = @lote";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@edicion", edicion);
                cmd.Parameters.AddWithValue("@lote", lote);

                return (long)cmd.ExecuteScalar() == 0;

            }
        }
        return false;
    }


}

