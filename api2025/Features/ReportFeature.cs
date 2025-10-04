using api2025.Entity;
using api2025.Enums;
using api2025.Services;
using EntityArchitect.CRUD.CustomEndpoints;
using EntityArchitect.CRUD.Entities.Context;
using EntityArchitect.CRUD.Entities.Repository;
using EntityArchitect.CRUD.Enumerations;
using EntityArchitect.CRUD.Results.Abstracts;

namespace api2025.Features;

public class ReportFeature(IRepository<Report> repository, IUnitOfWork unitOfWork, IXlsxService xlsxService) :  CustomEndpoint<Report>
{
    [CustomEndpoint("POST", "create")]
    public async Task<Result<string>> SaveReport(ReportRequest request, CancellationToken cancellationToken)
    {
        var entity = EntityArchitect.CRUD.Entities.Entities.Entity.CreateFromId<Report>(Guid.NewGuid());
        entity.UsageTime = DateTime.UtcNow;
        entity.ExpectedPension = request.ExpectedPension;
        entity.Pension = request.Pension;
        entity.PostalCode = request.PostalCode;
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
}