using api2025.Entity;
using EntityArchitect.CRUD.Queries;

namespace api2025.Queries.Definition;

public class GetAllByProvinceId() : Query<Report>("Queries/GetAllByProvince.sql", true);