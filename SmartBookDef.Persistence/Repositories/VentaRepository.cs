using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Persistence.Repositories;
public class VentaRepository : IVentaRepository
{
    private readonly string _connectionString;

    public VentaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("smart_book");
    }

    private string Sql { get; set; }
    private const string FORMATO_FECHA = "yyyy-MM-dd";

    public bool ValidarVentaIngreso(string libroLote)
    {

        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"SELECT COUNT(id) 
                    FROM venta_libros 
                    WHERE libroLote =@libro";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@libro", libroLote);

                return (long)cmd.ExecuteScalar() == 0;

            }
        }
    }

    public void Crear(VentaLibro venta)
    {


        using (var conexion = new MySqlConnection(_connectionString))
        {

            conexion.Open();

            Sql = @"INSERT INTO venta_libros VALUES(@id,@numero_recibo,@fecha,@libro,@unidades,@cliente,@usuario,@observaciones)";


            using (var cmd = new MySqlCommand(Sql, conexion))
            {

                cmd.Parameters.AddWithValue("@id", venta.Id);
                cmd.Parameters.AddWithValue("@numero_recibo", venta.NumeroReciboPago);
                cmd.Parameters.AddWithValue("@fecha", venta.Fecha.ToString(FORMATO_FECHA));
                cmd.Parameters.AddWithValue("@libro", venta.LibroLote);
                cmd.Parameters.AddWithValue("@unidades", venta.Unidades);
                cmd.Parameters.AddWithValue("@cliente", venta.Cliente);
                cmd.Parameters.AddWithValue("@usuario", venta.Usuario);
                cmd.Parameters.AddWithValue("@observaciones", venta.Observaciones);

                if (cmd.ExecuteNonQuery() > 0)
                {


                }

            }

        }


    }

    public bool DisminuirStock(string libroLote, int unidades)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();
            var sql = @"UPDATE libros
            SET stock = stock - @unidades
            WHERE id = @libro";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@unidades", unidades);
                cmd.Parameters.AddWithValue("@libro", libroLote);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public bool StockDisponible(string libroLote, int unidadesSolicitadas)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();
            var sql = @"SELECT stock FROM libros WHERE id = @libro";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@libro", libroLote);
                var resultado = cmd.ExecuteScalar();

                if (resultado == null)
                {
                    return false; 
                }

                int stockActual = Convert.ToInt32(resultado);
                return stockActual >= unidadesSolicitadas;
            }
        }
    }

    public decimal? PrecioLibro(string libroLote)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            var sql = @"SELECT ing.valor_venta_publico
            FROM libros lib
            INNER JOIN ingreso_libros ing ON lib.id = ing.libro
            WHERE lib.id = @librolote
            ORDER BY ing.fecha DESC
            LIMIT 1";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@librolote", libroLote);
                var result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToDecimal(result);
            }
        }
    }

    public string Usuario(string usuario)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();
            var sql = @"SELECT identificacion FROM usuarios WHERE identificacion = @identificacion";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@identificacion", usuario);
                var result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return null;

                return result.ToString();
            }
        }
    }

    public string CorreoCliente(string identificacionCliente)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();
            var sql = @"SELECT email_usuario FROM clientes WHERE identificacion = @identificacion";

            using (var cmd = new MySqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@identificacion", identificacionCliente);
                var result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return null;

                return result.ToString();
            }
        }
    }

    private static readonly object _lock = new();
    private static int _contador = 0;

    public string NumeroRecibo()
    {
        lock (_lock)
        {
            _contador = (_contador + 1) % 1000; 
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string seq = _contador.ToString("D3");
            return timestamp + seq; 
        }
    }

    public ConsultarVentaResponse? Consultar(string id)
    {
        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            Sql = @"SELECT 
                    vl.id,
                    vl.numero_recibo,
                    vl.fecha,
                    vl.libro,
                    vl.unidades,
                    vl.cliente,
                    vl.usuario,
                    vl.observaciones,
                    c.email_usuario as correo_cliente,
                    COALESCE(ing.valor_venta_publico, 0) as precio_unitario
                FROM venta_libros vl
                INNER JOIN clientes c ON vl.cliente = c.identificacion
                LEFT JOIN ingreso_libros ing ON vl.libro = ing.libro
                WHERE vl.id = @id
                ORDER BY ing.fecha DESC
                LIMIT 1";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    reader.Read();

                    var numeroRecibo = reader["numero_recibo"].ToString();
                    var fecha = DateOnly.FromDateTime((DateTime)reader["fecha"]);
                    var libro = reader["libro"].ToString();
                    var unidades = Convert.ToInt32(reader["unidades"]);
                    var cliente = reader["cliente"].ToString();
                    var usuario = reader["usuario"].ToString();
                    var observaciones = reader["observaciones"].ToString();
                    var correoCliente = reader["correo_cliente"].ToString();
                    var precioUnitario = Convert.ToDecimal(reader["precio_unitario"]);
                    var total = unidades * precioUnitario;

                    return new ConsultarVentaResponse(
                        id,
                        numeroRecibo,
                        fecha,
                        libro,
                        unidades,
                        cliente,
                        usuario,
                        observaciones,
                        correoCliente,
                        precioUnitario,
                        total
                    );

                }

            }
        }

    }

    public IEnumerable<ConsultarVentaResponse> Consultar()
    {
        var ventas = new List<ConsultarVentaResponse>();

        using (var conexion = new MySqlConnection(_connectionString))
        {
            conexion.Open();

            Sql = @"SELECT 
                    vl.id,
                    vl.numero_recibo,
                    vl.fecha,
                    vl.libro,
                    vl.unidades,
                    vl.cliente,
                    vl.usuario,
                    vl.observaciones,
                    c.email_usuario as correo_cliente,
                    COALESCE(ing.valor_venta_publico, 0) as precio_unitario
                FROM venta_libros vl
                INNER JOIN clientes c ON vl.cliente = c.identificacion
                LEFT JOIN ingreso_libros ing ON vl.libro = ing.libro
                ORDER BY vl.fecha DESC, vl.numero_recibo DESC";

            using (var cmd = new MySqlCommand(Sql, conexion))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader["id"].ToString();
                        var numeroRecibo = reader["numero_recibo"].ToString();
                        var fecha = DateOnly.FromDateTime((DateTime)reader["fecha"]);
                        var libro = reader["libro"].ToString();
                        var unidades = Convert.ToInt32(reader["unidades"]);
                        var cliente = reader["cliente"].ToString();
                        var usuario = reader["usuario"].ToString();
                        var observaciones = reader["observaciones"].ToString();
                        var correoCliente = reader["correo_cliente"].ToString();
                        var precioUnitario = Convert.ToDecimal(reader["precio_unitario"]);
                        var total = unidades * precioUnitario;

                        var venta = new ConsultarVentaResponse(
                            id,
                            numeroRecibo,
                            fecha,
                            libro,
                            unidades,
                            cliente,
                            usuario,
                            observaciones,
                            correoCliente,
                            precioUnitario,
                            total
                        );

                        ventas.Add(venta);
                    }
                }
            }
        }

        return ventas;
    }
}
