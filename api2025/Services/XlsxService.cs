using ClosedXML.Excel;

namespace api2025.Services;

public class XlsxService : IXlsxService
{
    public Task<string> GenerateXlsxReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");
        worksheet.Cell(1 , 1).Value = "Godzina użycia";
        worksheet.Cell(2 , 1).Value = "Emerytura oczekiwana";
        worksheet.Cell(3 , 1).Value = "Płeć";
        worksheet.Cell(4 , 1).Value = "Wysokość wynagrodzenia";
        worksheet.Cell(5 , 1).Value = "Czy uwzględniał okresy choroby";
        worksheet.Cell(6 , 1).Value = "Wysokość zgromadzonych środków na koncie";
        worksheet.Cell(7 , 1).Value = "Wysokość zgromadzonych środków na subkoncie";
        worksheet.Cell(8 , 1).Value = "Emerytura rzeczywista";
        worksheet.Cell(9 , 1).Value = "Emerytura urealniona";
        worksheet.Cell(10, 1).Value = "Kod pocztowy";
        
        
        worksheet.Cell(1, 2).Value = DateTime.Now;
        worksheet.Cell(2, 2).Value = request.ExpectedPension;
        worksheet.Cell(3, 2).Value = request.Sex == 0 ? "Mężczyzna" : (request.Sex == 1 ? "Kobieta" : throw new Exception("Niepoprawna wartość płci"));
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
}