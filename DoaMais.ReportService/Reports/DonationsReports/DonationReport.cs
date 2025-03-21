using DoaMais.ReportService.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DoaMais.ReportService.Reports.DonationsReports
{
    public class DonationReport : IDocument
    {
        public IEnumerable<Donation> Donations { get; }

        public DonationReport(IEnumerable<Donation> donations)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Donations = donations;
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

                        col.Item().Text("Relatório de Doações - Últimos 30 Dias")
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
                            columns.RelativeColumn(2); // ID do Doador
                            columns.RelativeColumn(3); // Nome do Doador
                            columns.RelativeColumn(2); // Data da Doação
                            columns.RelativeColumn(2); // Quantidade doada
                            columns.RelativeColumn(2); // Fator RH
                            columns.RelativeColumn(2); // Tipo Sanguíneo
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("ID do Doador").Bold().FontSize(12);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("Nome do Doador").Bold().FontSize(12);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("Data da Doação").Bold().FontSize(12);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("Quantidade (ml)").Bold().FontSize(12);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("Fator RH").Bold().FontSize(12);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("Tipo Sanguíneo").Bold().FontSize(12);
                        });

                        foreach (var donation in Donations.OrderByDescending(d => d.DonationDate))
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(donation.DonorId.ToString()).FontSize(11);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(donation.Donor.Name).FontSize(11);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(donation.DonationDate.ToString("dd/MM/yyyy")).FontSize(11);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text($"{donation.QuantityML} ml").FontSize(11);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(donation.Donor.RHFactor.ToString()).FontSize(11);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(donation.Donor.BloodType.ToString()).FontSize(11);

                            table.Cell().ColumnSpan(6).Height(10);
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
