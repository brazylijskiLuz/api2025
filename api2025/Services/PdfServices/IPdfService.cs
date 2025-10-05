namespace api2025.Services.PdfServices;

public interface IPdfService
{
    Task<string> CreatePdfReport(ReportRequest reportRequest, CancellationToken cancellationToken);
}