using api2025.Entity;
using EntityArchitect.CRUD.Entities.Context;
using EntityArchitect.CRUD.Entities.Repository;
using Microsoft.EntityFrameworkCore;

namespace api2025.Repositories;

public class PostCodeRepository(ApplicationDbContext context) : Repository<PostCode>(context), IPostCodeRepository
{
    public Task<PostCode?> GetPostCodesByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return context.Set<PostCode>()
            .Include(c => c.Province)
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }
}