using api2025.Entity;
using EntityArchitect.CRUD.Entities.Context;
using EntityArchitect.CRUD.Entities.Repository;
using Microsoft.EntityFrameworkCore;

namespace api2025.Repositories;

public class ReportRepository(ApplicationDbContext context) : Repository<Entity.Report>(context), IReportRepository
{
    public Task<List<Report>> GetReportsFromDateToDateAsync(DateTime? from, DateTime? to,
        CancellationToken cancellationToken) => 
        context.Set<Report>()
            .Include(c => c.PostalCode)
            .ThenInclude(c => c.Province)
            .Where(r => (!from.HasValue || r.UsageTime >= from) && (!to.HasValue || r.UsageTime <= to)).ToListAsync(cancellationToken: cancellationToken);
}