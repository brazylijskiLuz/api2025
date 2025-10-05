using System.Net;
using api2025.Entity;
using api2025.Enums;
using api2025.Repositories;
using api2025.Services;
using api2025.Services.PdfServices;
using EntityArchitect.CRUD.CustomEndpoints;
using EntityArchitect.CRUD.Entities.Context;
using EntityArchitect.CRUD.Enumerations;
using EntityArchitect.CRUD.Results.Abstracts;

namespace api2025.Features;

public class ReportFeature(IPdfService pdfService, IReportRepository repository, IPostCodeRepository postCodeRepository, IUnitOfWork unitOfWork, IXlsxService xlsxService) :  CustomEndpoint<Report>
{
    [CustomEndpoint("POST", "create")]
    public async Task<Result<FileResult>> SaveReport(ReportRequest request, CancellationToken cancellationToken)
    {
        PostCode? postCode = null;
        if (request.PostalCode is not null)
        {
            postCode = await postCodeRepository.GetPostCodesByCodeAsync(request.PostalCode.Replace(" ", ""),
                cancellationToken);
        }

        var entity = EntityArchitect.CRUD.Entities.Entities.Entity.CreateFromId<Report>(Guid.NewGuid());
        entity.UsageTime = DateTime.UtcNow.AddHours(2);
        entity.ExpectedPension = request.ExpectedPension;
        entity.Pension = request.Pension;
        entity.PostalCode = postCode;
        entity.RealPension = request.RealPension;
        entity.SalaryAmount = request.SalaryAmount;
        entity.Sex = Enumeration.GetById<Sex>(request.Sex);
        entity.ConsideredSickLeave = request.ConsideredSickLeave;
        entity.AccountBalance = request.AccountBalance;
        entity.Age = request.Age;
        entity.SubAccountBalance = request.SubAccountBalance;
        await repository.AddAsync(entity, cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var xlsxFile = await xlsxService.GenerateXlsxReportAsync(request, cancellationToken);
        var pdfFile = await pdfService.CreatePdfReport(request, cancellationToken);
        return new FileResult
        {
            PdfFile = pdfFile,
            XlsxFile = xlsxFile
        };
    }
    
    [CustomEndpoint("POST", "report-from-to")]
    public async Task<Result<string>> GetReportsFromDateToDateAsync(FromToRequest request, CancellationToken cancellationToken)
    {
        var from = DateTime.TryParse(request.From, out var fromDate)
            ? DateTime.SpecifyKind(fromDate, DateTimeKind.Utc)
            : (DateTime?)null;

        var to = DateTime.TryParse(request.To, out var toDate)
            ? DateTime.SpecifyKind(toDate, DateTimeKind.Utc)
            : (DateTime?)null;
        
        var reports = await repository.GetReportsFromDateToDateAsync(from, to, request.ProvinceId, cancellationToken);
        if(reports.Count == 0)
            return Result.Failure<string>(new Error(HttpStatusCode.NotFound, "No reports found in the given date range."));

        
        var filePath = await xlsxService.GenerateXlsxReportsFromDateToDateAsync(reports, cancellationToken);
        
        return filePath!;
    }
}