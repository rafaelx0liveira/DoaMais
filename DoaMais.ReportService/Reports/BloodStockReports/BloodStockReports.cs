using DoaMais.ReportService.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DoaMais.ReportService.Reports.BloodStockReports
{
    public class BloodStockReport : IDocument
    {
        public IEnumerable<BloodStock> BloodStockList { get; }

        public BloodStockReport(IEnumerable<BloodStock> bloodStockList)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            BloodStockList = bloodStockList;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header()
                    .Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .AlignLeft()
                                .Text("DoaMais")
                                .FontSize(22)
                                .Bold()
                                .FontColor(Colors.Red.Medium);
                        });

                        col.Item().Text("Relatório de Estoque de Sangue")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();

                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Black);
                    });


                page.Content()
                    .PaddingVertical(20)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Tipo Sanguíneo
                            columns.RelativeColumn(2); // Fator RH
                            columns.RelativeColumn(3); // Quantidade disponível
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Tipo Sanguíneo").Bold().FontSize(12);
                            header.Cell().Text("Fator RH").Bold().FontSize(12);
                            header.Cell().Text("Quantidade (ml)").Bold().FontSize(12);
                        });

                        foreach (var stock in BloodStockList.OrderBy(s => s.BloodType))
                        {
                            table.Cell().Text(stock.BloodType.ToString()).FontSize(11);
                            table.Cell().Text(stock.RHFactor.ToString()).FontSize(11);
                            table.Cell().Text($"{stock.QuantityML} ml").FontSize(11);
                        }
                    });

                page.Footer()
                    .AlignRight()
                    .Text($"Gerado em {DateTime.UtcNow.AddHours(-3):dd/MM/yyyy HH:mm}")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Medium);
            });
        }
    }
}
