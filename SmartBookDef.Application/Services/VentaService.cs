using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartBookDef.Application.Options;
using SmartBookDef.Domain.Dtos.Requests;
using SmartBookDef.Domain.Dtos.Responses;
using SmartBookDef.Domain.Entities;
using SmartBookDef.Domain.Exceptions;
using SmartBookDef.Persistence.Repositories.Interfaces;

namespace SmartBookDef.Application.Services;
public class VentaService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly PdfService _pdfService;
    private readonly Correo _correo;


    public VentaService(IVentaRepository ventaRepository, IOptions<Correo> options)
    {
        _ventaRepository = ventaRepository;
        _pdfService = new PdfService();
        _correo = options.Value;
    }

    public CrearVentaResponse CrearVentaYEnviarPdf(CrearVentaRequest request)
    {
        var usuario = _ventaRepository.Usuario(request.Usuario);
        if (string.IsNullOrEmpty(usuario))
        {
            throw new BussinesRuleException("No se encontró un Usuario con esta Identificación");
        }

        if (request.Libros == null || !request.Libros.Any())
        {
            throw new BussinesRuleException("Debe incluir al menos un libro en la venta");
        }

        foreach (var item in request.Libros)
        {
            if (item.Unidades <= 0)
            {
                throw new BussinesRuleException("La cantidad de unidades debe ser mayor a cero");
            }
        }
        var correoCliente = _ventaRepository.CorreoCliente(request.Cliente);
        if (string.IsNullOrEmpty(correoCliente))
        {
            throw new BussinesRuleException("El cliente con esa identificación no existe o no tiene un correo registrado");
        }

        var ventasConPrecio = new List<(VentaLibro venta, decimal precio)>();

        foreach (var libroItem in request.Libros)
        {

            var valorUnitario = _ventaRepository.PrecioLibro(libroItem.LibroLote);
            if (valorUnitario == null)
            {
                throw new BussinesRuleException($"El libro {libroItem.LibroLote} no tiene un precio asignado");
            }


            if (!_ventaRepository.StockDisponible(libroItem.LibroLote, libroItem.Unidades))
            {
                throw new BussinesRuleException($"Stock insuficiente ");
            }

            ventasConPrecio.Add((null, valorUnitario.Value));
        }

        var recibo = _ventaRepository.NumeroRecibo();

        var ventas = new List<VentaLibro>();
        for (int i = 0; i < request.Libros.Count; i++)
        {
            var libroItem = request.Libros[i];
            var venta = new VentaLibro
            {
                Id = $"{DateTime.Now.Ticks}{i}",
                NumeroReciboPago = recibo,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                LibroLote = libroItem.LibroLote,
                Unidades = libroItem.Unidades,
                Cliente = request.Cliente,
                Usuario = request.Usuario,
                Observaciones = request.Observaciones
            };

            ventas.Add(venta);
            ventasConPrecio[i] = (venta, ventasConPrecio[i].precio);
        }

        var pdfBytes = _pdfService.GenerarVentaMultiplePdf(ventasConPrecio, recibo, request.Cliente, request.Usuario, request.Observaciones);

        EnviarCorreoConPdfMailKit(correoCliente, pdfBytes, recibo);

        foreach (var venta in ventas)
        {
            _ventaRepository.Crear(venta);
        }

        foreach (var libroItem in request.Libros)
        {
            var actualizado = _ventaRepository.DisminuirStock(libroItem.LibroLote, libroItem.Unidades);
            if (!actualizado)
            {
                throw new BussinesRuleException($"Error al actualizar stock del libro {libroItem.LibroLote}");
            }
        }

        var primeraVenta = ventas.First();
        return new CrearVentaResponse(
            primeraVenta.Id,
            primeraVenta.NumeroReciboPago,
            primeraVenta.Fecha,
            primeraVenta.LibroLote,
            primeraVenta.Unidades,
            primeraVenta.Cliente,
            primeraVenta.Usuario,
            primeraVenta.Observaciones
        );
    }

    private void EnviarCorreoConPdfMailKit(string correoDestino, byte[] pdfBytes, string numeroRecibo)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress("SmartBook", _correo.cuenta));
        mensaje.To.Add(new MailboxAddress("", correoDestino));
        mensaje.Subject = $"Recibo de venta {numeroRecibo}";

        var body = new BodyBuilder
        {
            HtmlBody = "<p>Adjunto encontrará el PDF con el detalle de su compra.</p>"
        };

        body.Attachments.Add($"Recibo_{numeroRecibo}.pdf", pdfBytes, new ContentType("application", "pdf"));

        mensaje.Body = body.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.Connect("smtp.gmail.com", _correo.host, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(_correo.cuenta, _correo.contrasenia);
            client.Send(mensaje);
            client.Disconnect(true);
        }
    }
    public ConsultarVentaResponse? Consultar(string id)
    {
        return _ventaRepository.Consultar(id);
    }

    public IEnumerable<ConsultarVentaResponse> Consultar(ConsultarVentaRequest request)
    {
        var ventas = _ventaRepository.Consultar().AsEnumerable();

        if (request.Desde is not null)
            ventas = ventas.Where(i => i.Fecha >= request.Desde);

        if (request.Hasta is not null)
            ventas = ventas.Where(i => i.Fecha <= request.Hasta);

        if (request.Cliente is not null)
            ventas = ventas.Where(i => i.Cliente == request.Cliente);

        if (request.LibroLote is not null)
            ventas = ventas.Where(i => i.LibroLote == request.LibroLote);

        return ventas.Select(i => new ConsultarVentaResponse(


            i.Id,
            i.NumeroReciboPago,
            i.Fecha,
            i.LibroLote,
            i.Unidades,
            i.Cliente,
            i.Usuario,
            i.Observaciones,
            i.CorreoCliente,
            i.PrecioUnitario,
            i.Total

   ));
    }

}
