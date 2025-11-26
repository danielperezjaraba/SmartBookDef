using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartBookDef.Domain.Entities;

namespace SmartBookDef.Application.Services;
public class PdfService
{
    private readonly string _logoPath = @"C:\Users\danie\Downloads\Imagen de WhatsApp 2025-11-22 a las 17.08.59_9a511938.jpg";

    public PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerarVentaMultiplePdf(List<(VentaLibro venta, decimal precio)> ventasConPrecio, string numeroRecibo, string cliente, string usuario, string observaciones)
    {
        var pdfBytes = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(30);


                page.Header().ShowOnce().Row(row =>
                {
                    row.ConstantItem(140).Height(60).Column(col =>
                    {
                        if (File.Exists(_logoPath))
                        {
                            col.Item().AlignCenter().AlignMiddle()
                                .Image(_logoPath)
                                .FitArea();
                        }
                        else
                        {
                            col.Item().AlignCenter().AlignMiddle()
                                .Text("LOGO").FontSize(20).Bold();
                        }
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignCenter().Text("SmartBook SAC").Bold().FontSize(14);
                        col.Item().AlignCenter().Text("Calle Falsa 123, Ciudad").FontSize(9);
                        col.Item().AlignCenter().Text("Tel: 300 123 4567").FontSize(9);
                        col.Item().AlignCenter().Text("contacto@smartbook.com").FontSize(9);
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Border(1).BorderColor("#257272")
                            .AlignCenter().Text("RUC 123456789");

                        col.Item().Background("#257272").Border(1)
                            .BorderColor("#257272").AlignCenter()
                            .Text("Boleta de venta").FontColor("#fff");

                        col.Item().Border(1).BorderColor("#257272")
                            .AlignCenter().Text(numeroRecibo);
                    });
                });


                page.Content().PaddingVertical(10).Column(col1 =>
                {

                    col1.Item().Column(col2 =>
                    {
                        col2.Item().Text("Datos del cliente").Underline().Bold();

                        col2.Item().Text(txt =>
                        {
                            txt.Span("Cliente: ").SemiBold().FontSize(10);
                            txt.Span(cliente).FontSize(10);
                        });

                        col2.Item().Text(txt =>
                        {
                            txt.Span("Usuario: ").SemiBold().FontSize(10);
                            txt.Span(usuario).FontSize(10);
                        });

                        col2.Item().Text(txt =>
                        {
                            txt.Span("Fecha: ").SemiBold().FontSize(10);
                            txt.Span(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(10);
                        });
                    });

                    col1.Item().LineHorizontal(0.5f);


                    col1.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background("#257272").Padding(2).Text("Libro / Lote").FontColor("#fff");
                            header.Cell().Background("#257272").Padding(2).Text("Unidades").FontColor("#fff");
                            header.Cell().Background("#257272").Padding(2).Text("Precio Unit").FontColor("#fff");
                            header.Cell().Background("#257272").Padding(2).Text("Total").FontColor("#fff");
                        });


                        decimal totalGeneral = 0;
                        foreach (var item in ventasConPrecio)
                        {
                            var subtotal = item.venta.Unidades * item.precio;
                            totalGeneral += subtotal;

                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(item.venta.LibroLote);
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(item.venta.Unidades.ToString());
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text($"S/. {item.precio}");
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).AlignRight().Text($"S/. {subtotal}");
                        }
                    });


                    decimal total = ventasConPrecio.Sum(x => x.venta.Unidades * x.precio);
                    col1.Item().AlignRight().Text($"Total: S/. {total}").FontSize(12).Bold();


                    if (!string.IsNullOrEmpty(observaciones))
                        col1.Item().Background(Colors.Grey.Lighten3).Padding(10)
                            .Column(column =>
                            {
                                column.Item().Text("Observaciones").FontSize(14);
                                column.Item().Text(observaciones);
                                column.Spacing(5);
                            });

                    col1.Spacing(10);
                });

                page.Footer()
                    .AlignRight()
                    .Text(txt =>
                    {
                        txt.Span("Pagina ").FontSize(10);
                        txt.CurrentPageNumber().FontSize(10);
                        txt.Span(" de ").FontSize(10);
                        txt.TotalPages().FontSize(10);
                    });
            });
        }).GeneratePdf();

        return pdfBytes;
    }
}
