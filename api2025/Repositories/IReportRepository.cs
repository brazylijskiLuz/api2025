using api2025.Entity;
using EntityArchitect.CRUD.Entities.Repository;

namespace api2025.Repositories;

public interface IReportRepository : IRepository<Entity.Report>
{
    public Task<List<Report>> GetReportsFromDateToDateAsync(DateTime? from, DateTime? to,
        CancellationToken cancellationToken);
}