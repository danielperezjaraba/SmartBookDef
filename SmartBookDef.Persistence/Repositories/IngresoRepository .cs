using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Enums;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Persistence.Repositories;
public class IngresoRepository : IIngresoRepository
{
    private readonly string _connectionString;

    public IngresoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("smart_book");
    }

    private string Sql { get; set; }
    private const string FORMATO_FECHA = "yyyy-MM-dd";

    public bool ValidarCreacionIngreso(string libro, string lote)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            Sql = @"SELECT COUNT(id) 
                FROM ingreso_libros 
                WHERE libro = @libro AND lote = @lote";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@libro", libro);
                cmd.Parameters.AddWithValue("@lote", lote);

                return (long)cmd.ExecuteScalar() == 0;
            }
        }
    }

    public void Crear(IngresoLibro ingresoLibro)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"
                    INSERT INTO ingreso_libros VALUES(
                    @id, 
                    @fecha, 
                    @libro, 
                    @lote, 
                    @unidades, 
                    @valor_compra, 
                    @valor_venta_publico
                    )";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@id", ingresoLibro.Id);
                cmd.Parameters.AddWithValue("@fecha", ingresoLibro.Fecha.ToString(FORMATO_FECHA));
                cmd.Parameters.AddWithValue("@libro", ingresoLibro.Libro);
                cmd.Parameters.AddWithValue("@lote", ingresoLibro.Lote);
                cmd.Parameters.AddWithValue("@unidades", ingresoLibro.Unidades);
                cmd.Parameters.AddWithValue("@valor_compra", ingresoLibro.ValorCompra);
                cmd.Parameters.AddWithValue("@valor_venta_publico", ingresoLibro.ValorVentaPublico);


                if (cmd.ExecuteNonQuery() > 0)
                {


                }

            }

        }

    }

    public ConsultarIngresoResponse? Consultar(string id)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();
            Sql = @"SELECT * FROM ingreso_libros where 
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

                var fecha = DateOnly.FromDateTime((DateTime)reader["fecha"]);
                var libro = reader["libro"].ToString();
                var lote = reader["lote"].ToString();
                var unidades = Convert.ToInt32(reader["unidades"]);
                var valorCompra = Convert.ToDecimal(reader["valor_compra"]);
                var valorVentaPublico = Convert.ToDecimal(reader["valor_venta_publico"]);

                return new ConsultarIngresoResponse(

                    id,
                    fecha,
                    libro,
                    lote,
                    unidades,
                    valorCompra,
                    valorVentaPublico
                );

            }

        }
    }

    public IEnumerable<ConsultarIngresoResponse> Consultar()
    {
        var ingresos = new List<ConsultarIngresoResponse>();


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = "SELECT * FROM ingreso_libros";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                var reader = cmd.ExecuteReader();
                Sql = string.Empty;

                while (reader.Read())
                {
                    var id = reader["id"].ToString();
                    var fecha = DateOnly.FromDateTime((DateTime)reader["fecha"]);
                    var libro = reader["libro"].ToString();
                    var lote = reader["lote"].ToString();
                    var unidades = Convert.ToInt32(reader["unidades"]);
                    var valorCompra = Convert.ToDecimal(reader["valor_compra"]);
                    var valorVentaPublico = Convert.ToDecimal(reader["valor_venta_publico"]);


                    var ingreso = new ConsultarIngresoResponse
                    (

                        id,
                        fecha,
                        libro,
                        lote,
                        unidades,
                        valorCompra,
                        valorVentaPublico

                    );

                    ingresos.Add(ingreso);

                }
                return ingresos;

            }



        }
    }



    public bool ExisteLibro(string libro)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            var sql = @"SELECT COUNT(id) 
                    FROM libros 
                    WHERE id = @libro";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@libro", libro);

                return (long)cmd.ExecuteScalar() > 0;
            }
        }
    }

    public bool SumarStock(string libro, int unidades)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            var sql = @"UPDATE libros
                    SET stock = stock + @unidades
                    WHERE id = @libro";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@unidades", unidades);
                cmd.Parameters.AddWithValue("@libro", libro);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public string? ObtenerLotePorId(string id)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            var sql = @"SELECT lote FROM libros WHERE id = @id LIMIT 1";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@id", id);

                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return null;

                return result.ToString();
            }
        }
    }

}

