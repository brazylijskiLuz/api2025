using api2025.Entity;
using EntityArchitect.CRUD.Entities.Repository;

namespace api2025.Repositories;

public interface IPostCodeRepository : IRepository<PostCode>
{
    public Task<PostCode?> GetPostCodesByCodeAsync(string code, CancellationToken cancellationToken);
}