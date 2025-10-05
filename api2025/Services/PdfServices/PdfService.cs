using System.Globalization;
using api2025.Entity;
using api2025.Enums;
using api2025.Services.PdfServices.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
// ImageSharp jest pod spodem wykorzystywany przez ScottPlot 5, ale tu nie musimy go importować

namespace api2025.Services.PdfServices;

public class PdfService : IPdfService
{
    public Task<string> CreatePdfReport(ReportRequest reportRequest, CancellationToken cancellationToken)
    {
        // ✅ poprawka nazwy propsa
        var data = reportRequest.Data;
        var name = "/reports/" + Guid.NewGuid() + ".pdf";

        if (data == null || data.Count == 0)
            throw new ArgumentException("Lista scenariuszy jest pusta.");
        

        // wykres jako obraz w pamięci
        var chartStream = GenerujWykresPng(data);

        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(Naglowek("Raport przewidywanej emerytury"));

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().PaddingBottom(10).Element(StanAktualnyBox(reportRequest, CultureInfo.InvariantCulture));
                    col.Item().PaddingBottom(6).LineHorizontal(0.75f);
                    
                    if(data is null || data.Count == 0)
                        return;
                    
                    col.Item().PaddingTop(20).Text("Wykres: emerytura netto w czasie").SemiBold().FontSize(12);
                    col.Item().PaddingTop(8).AlignCenter().Image(chartStream.ToArray());
                    col.Item().PaddingVertical(6).LineHorizontal(0.5f);
                    
                    col.Item().PageBreak();
                    col.Item().Text("Pozostałe scenariusze").SemiBold().FontSize(12);

                    foreach (var s in data.OrderBy(x => x.rok_emerytury))
                    {
                        col.Item().Component(new DataBoxComponent(s, CultureInfo.InvariantCulture));
                    }


                });

                // Stopka
                page.Footer().AlignRight().Text($"Wygenerowano: {DateTime.UtcNow.AddHours(2):yyyy-MM-dd HH:mm}");
            });
        })
        .GeneratePdf("wwwroot" + name);

        return Task.FromResult("https://api.wnek.cc" + name);
    }

    private static Action<IContainer> Naglowek(string tytul) => container =>
    {
        container.Row(row =>
        {
            row.ConstantItem(10).Height(10);
            row.RelativeItem().Column(col =>
            {
                col.Item().Text(tytul).Bold().FontSize(16);
                col.Item().Text("Podsumowanie wariantów wydłużenia aktywności zawodowej");
            });
        });
    };

    // --- STAN AKTUALNY ---
    private static Action<IContainer> StanAktualnyBox(ReportRequest s, CultureInfo pl) => container =>
    {
        container
            .Padding(12)
            .Background(Colors.Grey.Lighten4)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Column(col =>
            {
                col.Spacing(4);

                // 🔹 Tytuł sekcji

                col.Item().Text("Stan Aktualny").Bold().FontSize(13);

                // Helper do par "etykieta: wartość"
                void Row(string etykieta, string wartosc)
                    => col.Item().Text(t =>
                    {
                        t.Span(etykieta + ": ").Bold();
                        t.Span(wartosc);
                    });

                // 🔹 Dane podstawowe
                col.Item().PaddingTop(6).Text("Dane podstawowe").SemiBold().FontSize(12);
                Row("Wiek", $"{s.Age} lat");
                Row("Płeć", s.Sex == Sex.Male.Id ? "mężczyzna" : "kobieta");
                Row("Data użycia kalkulatora", DateTime.Now.ToString("yyyy-MM-dd HH:mm", pl));

                // 🔹 Finanse
                col.Item().PaddingTop(6).Text("Dane finansowe").SemiBold().FontSize(12);
                Row("Wynagrodzenie miesięczne", s.SalaryAmount.ToString("N2", pl) + " zł");
                Row("Uwzględniono L4", s.ConsideredSickLeave ? "Tak" : "Nie");
                Row("Saldo konta w ZUS", s.AccountBalance.ToString("N2", pl) + " zł");
                Row("Saldo subkonta w ZUS", s.SubAccountBalance.ToString("N2", pl) + " zł");

                // 🔹 Emerytura
                col.Item().PaddingTop(6).Text("Emerytura").SemiBold().FontSize(12);
                Row("Prognozowana emerytura nominalna", s.Pension.ToString("N2", pl) + " zł");
                Row("Prognozowana emerytura realna", s.RealPension.ToString("N2", pl) + " zł");
                Row("Oczekiwana emerytura (po wydłużeniu pracy)", s.ExpectedPension.ToString("N2", pl) + " zł");
            });
    };


    // --- WYKRES (ScottPlot 5) ---
    private static MemoryStream GenerujWykresPng(List<PdfChartModel> scenariusze)
    {
        var posortowane = scenariusze.OrderBy(s => s.rok_emerytury).ToList();

        double[] x = posortowane.Select(s => (double)s.rok_emerytury).ToArray();
        double[] yNomNetto = posortowane.Select(s => s.emerytura_nominalna_netto).ToArray();
        double[] yUrealNetto = posortowane.Select(s => s.emerytura_urealniona_netto).ToArray();

        var plt = new ScottPlot.Plot();

        var sp1 = plt.Add.Scatter(x, yNomNetto);  sp1.LegendText = "Nominalna NETTO";
        var sp2 = plt.Add.Scatter(x, yUrealNetto); sp2.LegendText = "Urealniona NETTO";

        plt.Axes.Bottom.Label.Text = "Rok emerytury";
        plt.Axes.Left.Label.Text = "Kwota [zł]";
        plt.Legend.Location = ScottPlot.Alignment.UpperRight;
        plt.Legend.Orientation = ScottPlot.Orientation.Vertical;
        plt.Legend.BackgroundColor = ScottPlot.Colors.White;
        plt.Legend.OutlineColor = ScottPlot.Colors.Black;
        plt.Legend.FontSize = 12;

        plt.Axes.Frame(true);
        plt.Title("Emerytura NETTO – nominalna vs urealniona");
        plt.ShowLegend();

        // etykiety co 1 rok
        plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(1);

        // W ScottPlot 5: GetImage/Render zwraca ImageSharp.Image
        var image = plt.GetImage(900, 450);

        // Prosty zapis do pliku tymczasowego i wczytanie do streamu
        var tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
        image.Save(tmp);

        var ms = new MemoryStream(File.ReadAllBytes(tmp));
        ms.Position = 0;

        try { File.Delete(tmp); } catch { /* nieistotne */ }

        return ms;
    }
}
