using api2025.Entity;
using EntityArchitect.CRUD.Queries;

namespace api2025.Queries.Definition;

public class GetAllProvinces() : Query<Province>("Queries/GetAllProvinces.sql", true);