using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Persistence.Repositories;
public class InventarioRepository : IInventarioRepository
{
    private readonly string _connectionString;

    public InventarioRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("smart_book");
    }
    private string Sql { get; set; }
    public IEnumerable<ConsultarInventarioResponse> ConsultarInventarioPorLote(string lote)
    {
        var lista = new List<ConsultarInventarioResponse>();

        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            Sql = @"
            SELECT 
                l.nombre,
                l.tipo,
                SUM(i_l.unidades) AS unidades_ingresadas,
                SUM(v_l.unidades) AS unidades_vendidas,
                l.stock
            FROM libros l
            JOIN ingreso_libros i_l ON l.id = i_l.libro
            JOIN venta_libros v_l ON l.id = v_l.libro
            WHERE l.lote = @lote
            GROUP BY l.id, l.nombre, l.tipo, l.stock
        ";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@lote", lote);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new ConsultarInventarioResponse(
                        reader["nombre"].ToString()!,
                        reader["tipo"].ToString()!,
                        Convert.ToInt32(reader["unidades_ingresadas"]),
                        Convert.ToInt32(reader["unidades_vendidas"]),
                        Convert.ToInt32(reader["stock"])
                    ));
                }
            }
        }

        return lista;
    }
}
