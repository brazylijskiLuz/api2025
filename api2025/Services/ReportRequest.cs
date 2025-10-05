using api2025.Services.PdfServices;

namespace api2025.Services;

public class ReportRequest
{
    public decimal ExpectedPension { get; set; }
    public int Sex { get; set; }
    public decimal SalaryAmount { get; set; }
    public bool ConsideredSickLeave { get; set; }
    public decimal AccountBalance { get; set; }
    public decimal SubAccountBalance { get; set; }
    public decimal Pension { get; set; }
    public decimal RealPension { get; set; }
    public int Age { get; set; }
    public string? PostalCode { get; set; } = null;
    
    public List<PdfChartModel>? Data { get; set; } = new();
}