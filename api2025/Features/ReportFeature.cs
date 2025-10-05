using System.Net;
using api2025.Entity;
using api2025.Enums;
using api2025.Repositories;
using api2025.Services;
using EntityArchitect.CRUD.CustomEndpoints;
using EntityArchitect.CRUD.Entities.Context;
using EntityArchitect.CRUD.Enumerations;
using EntityArchitect.CRUD.Results.Abstracts;

namespace api2025.Features;

public class ReportFeature(IReportRepository repository, IPostCodeRepository postCodeRepository, IUnitOfWork unitOfWork, IXlsxService xlsxService) :  CustomEndpoint<Report>
{
    [CustomEndpoint("POST", "create")]
    public async Task<Result<string>> SaveReport(ReportRequest request, CancellationToken cancellationToken)
    {
        PostCode? postCode = null;
        if (request.PostalCode is not null)
        {
            postCode = await postCodeRepository.GetPostCodesByCodeAsync(request.PostalCode.Replace(" ", ""),
                cancellationToken);
            if(postCode is null)
                return Result.Failure<string>(new Error(HttpStatusCode.BadRequest, "Invalid postal code."));
        }

        var entity = EntityArchitect.CRUD.Entities.Entities.Entity.CreateFromId<Report>(Guid.NewGuid());
        entity.UsageTime = DateTime.UtcNow;
        entity.ExpectedPension = request.ExpectedPension;
        entity.Pension = request.Pension;
        entity.PostalCode = postCode;
        entity.RealPension = request.RealPension;
        entity.SalaryAmount = request.SalaryAmount;
        entity.Sex = Enumeration.GetById<Sex>(request.Sex);
        entity.ConsideredSickLeave = request.ConsideredSickLeave;
        entity.AccountBalance = request.AccountBalance;
        entity.SubAccountBalance = request.SubAccountBalance;
        await repository.AddAsync(entity, cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var filePath = await xlsxService.GenerateXlsxReportAsync(request, cancellationToken);
        return filePath!;
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
        
        var reports = await repository.GetReportsFromDateToDateAsync(from, to, cancellationToken);
        if(reports.Count == 0)
            return Result.Failure<string>(new Error(HttpStatusCode.NotFound, "No reports found in the given date range."));

        
        var filePath = await xlsxService.GenerateXlsxReportsFromDateToDateAsync(reports, cancellationToken);
        
        return filePath!;
    }
}