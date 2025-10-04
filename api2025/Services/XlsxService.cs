using ClosedXML.Excel;

namespace api2025.Services;

public class XlsxService : IXlsxService
{
    public Task<string> GenerateXlsxReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("raport");
        worksheet.Cell(1, 1).Value = "Godzina użycia";
        worksheet.Cell(2, 1).Value = "Emerytura oczekiwana";
        worksheet.Cell(3, 1).Value = "Płeć";
        worksheet.Cell(4, 1).Value = "Wysokość wynagrodzenia";
        worksheet.Cell(5, 1).Value = "Czy uwzględniał okresy choroby";
        worksheet.Cell(6, 1).Value = "Wysokość zgromadzonych środków na koncie";
        worksheet.Cell(7, 1).Value = "Wysokość zgromadzonych środków na subkoncie";
        worksheet.Cell(8, 1).Value = "Emerytura rzeczywista";
        worksheet.Cell(9, 1).Value = "Emerytura urealniona";
        worksheet.Cell(10, 1).Value = "Kod pocztowy";


        worksheet.Cell(1, 2).Value = DateTime.Now;
        worksheet.Cell(2, 2).Value = request.ExpectedPension;
        worksheet.Cell(3, 2).Value = request.Sex == 0
            ? "Mężczyzna"
            : (request.Sex == 1 ? "Kobieta" : throw new Exception("Niepoprawna wartość płci"));
        worksheet.Cell(4, 2).Value = request.SalaryAmount;
        worksheet.Cell(5, 2).Value = request.ConsideredSickLeave ? "Tak" : "Nie";
        worksheet.Cell(6, 2).Value = request.AccountBalance;
        worksheet.Cell(7, 2).Value = request.SubAccountBalance;
        worksheet.Cell(8, 2).Value = request.Pension;
        worksheet.Cell(9, 2).Value = request.RealPension;
        worksheet.Cell(10, 2).Value = request.PostalCode ?? "Brak danych";

        worksheet.Columns().AdjustToContents();

        var name = "reports/" + Guid.NewGuid() + ".xlsx";
        workbook.SaveAs("wwwroot/" + name);

        return Task.FromResult("api.wnek.cc/" + name);
    }

    public Task<string> GenerateXlsxReportsFromDateToDateAsync(List<ReportRequest> request,
        CancellationToken cancellationToken)
    {
        using var workbook = new XLWorkbook();

        var wsSummary = workbook.Worksheets.Add("Podsumowanie");

        wsSummary.Cell(1, 1).Value = "Raport z systemu emerytalnego";
        wsSummary.Range("A1:B1").Merge().Style
            .Font.SetBold()
            .Font.FontSize = 16;

        wsSummary.Cell(2, 1).Value = "Godzina użycia:";
        wsSummary.Cell(2, 2).Value = DateTime.Now;
        wsSummary.Cell(3, 1).Value = "Liczba rekordów:";
        wsSummary.Cell(3, 2).Value = request.Count;

        int row = 5;

        // 🔹 Statystyki ogólne
        wsSummary.Cell(row++, 1).Value = "Statystyki ogólne";
        wsSummary.Cell(row, 1).Value = "Średnia pensja";
        wsSummary.Cell(row++, 2).Value = request.Average(x => x.SalaryAmount);
        wsSummary.Cell(row, 1).Value = "Średnia oczekiwana emerytura";
        wsSummary.Cell(row++, 2).Value = request.Average(x => x.ExpectedPension);
        wsSummary.Cell(row, 1).Value = "Średnia realna emerytura";
        wsSummary.Cell(row++, 2).Value = request.Average(x => x.RealPension);
        wsSummary.Cell(row, 1).Value = "Średni stosunek realnej do oczekiwanej";
        wsSummary.Cell(row++, 2).Value =
            request.Average(x => x.RealPension / (x.ExpectedPension == 0 ? 1 : x.ExpectedPension));

        // 🔹 Statystyki min / max
        wsSummary.Cell(row++, 1).Value = "Statystyki minimalne / maksymalne";
        wsSummary.Cell(row, 1).Value = "Najniższa pensja";
        wsSummary.Cell(row++, 2).Value = request.Min(x => x.SalaryAmount);
        wsSummary.Cell(row, 1).Value = "Najwyższa pensja";
        wsSummary.Cell(row++, 2).Value = request.Max(x => x.SalaryAmount);
        wsSummary.Cell(row, 1).Value = "Najniższa oczekiwana emerytura";
        wsSummary.Cell(row++, 2).Value = request.Min(x => x.ExpectedPension);
        wsSummary.Cell(row, 1).Value = "Najwyższa oczekiwana emerytura";
        wsSummary.Cell(row++, 2).Value = request.Max(x => x.ExpectedPension);
        wsSummary.Cell(row, 1).Value = "Najniższy stan konta (łącznie)";
        wsSummary.Cell(row++, 2).Value = request.Min(x => x.AccountBalance + x.SubAccountBalance);
        wsSummary.Cell(row, 1).Value = "Najwyższy stan konta (łącznie)";
        wsSummary.Cell(row++, 2).Value = request.Max(x => x.AccountBalance + x.SubAccountBalance);

        // 🔹 Statystyki płci
        int women = request.Count(x => x.Sex == 0);
        int men = request.Count(x => x.Sex == 1);

        wsSummary.Cell(row++, 1).Value = "Statystyki według płci";
        wsSummary.Cell(row, 1).Value = "Kobiety (liczba)";
        wsSummary.Cell(row++, 2).Value = women;
        wsSummary.Cell(row, 1).Value = "Mężczyźni (liczba)";
        wsSummary.Cell(row++, 2).Value = men;
        wsSummary.Cell(row, 1).Value = "Procent kobiet";
        wsSummary.Cell(row++, 2).Value = (decimal)women / request.Count * 100;
        wsSummary.Cell(row, 1).Value = "Procent mężczyzn";
        wsSummary.Cell(row++, 2).Value = (decimal)men / request.Count * 100;

        wsSummary.Cell(row++, 1).Value = "Urlopy chorobowe";
        wsSummary.Cell(row, 1).Value = "Osoby z uwzględnionym chorobowym";
        wsSummary.Cell(row++, 2).Value = request.Count(x => x.ConsideredSickLeave);
        wsSummary.Cell(row, 1).Value = "Osoby bez chorobowego";
        wsSummary.Cell(row++, 2).Value = request.Count(x => !x.ConsideredSickLeave);

        // 🔹 Grupy po kodzie pocztowym
        wsSummary.Cell(row++, 1).Value = "Podział po kodach pocztowych";
        var postalGroups = request
            .Where(x => !string.IsNullOrEmpty(x.PostalCode))
            .GroupBy(x => x.PostalCode!)
            .Select(g => new
            {
                Postal = g.Key,
                Count = g.Count(),
                AvgSalary = g.Average(x => x.SalaryAmount),
                AvgExpected = g.Average(x => x.ExpectedPension)
            })
            .OrderByDescending(g => g.Count);

        row += 1;
        wsSummary.Cell(row, 1).Value = "Kod pocztowy";
        wsSummary.Cell(row, 2).Value = "Liczba osób";
        wsSummary.Cell(row, 3).Value = "Średnia pensja";
        wsSummary.Cell(row++, 4).Value = "Średnia oczekiwana emerytura";

        foreach (var g in postalGroups)
        {
            wsSummary.Cell(row, 1).Value = g.Postal;
            wsSummary.Cell(row, 2).Value = g.Count;
            wsSummary.Cell(row, 3).Value = g.AvgSalary;
            wsSummary.Cell(row++, 4).Value = g.AvgExpected;
        }

        wsSummary.Columns().AdjustToContents();

        var wsDetails = workbook.Worksheets.Add("Szczegóły");

        int dRow = 1;
        var headers = new[]
        {
            "Lp", "Płeć", "Pensja", "Oczekiwana emerytura", "Rzeczywista emerytura",
            "Stan konta", "Subkonto", "Chorobowe", "Kod pocztowy"
        };

        for (int i = 0; i < headers.Length; i++)
            wsDetails.Cell(dRow, i + 1).Value = headers[i];

        dRow++;

        var index = 1;
        foreach (var r in request)
        {
            wsDetails.Cell(dRow, 1).Value = index++;
            wsDetails.Cell(dRow, 2).Value = r.Sex == 0 ? "Kobieta" : "Mężczyzna";
            wsDetails.Cell(dRow, 3).Value = r.SalaryAmount;
            wsDetails.Cell(dRow, 4).Value = r.ExpectedPension;
            wsDetails.Cell(dRow, 5).Value = r.RealPension;
            wsDetails.Cell(dRow, 6).Value = r.AccountBalance;
            wsDetails.Cell(dRow, 7).Value = r.SubAccountBalance;
            wsDetails.Cell(dRow, 8).Value = r.ConsideredSickLeave ? "Tak" : "Nie";
            wsDetails.Cell(dRow++, 9).Value = r.PostalCode ?? "-";
        }

        wsDetails.Columns().AdjustToContents();

        var name = "reports/" + Guid.NewGuid() + ".xlsx";
        workbook.SaveAs("wwwroot/" + name);

        return Task.FromResult("api.wnek.cc/" + name);
    }
}