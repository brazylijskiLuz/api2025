namespace api2025.Services;

public interface IXlsxService
{
    Task<string> GenerateXlsxReportAsync(ReportRequest request, CancellationToken cancellationToken);
    Task<string> GenerateXlsxReportsFromDateToDateAsync(List<ReportRequest> request, CancellationToken cancellationToken);
    
}