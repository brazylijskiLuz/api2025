using api2025.Entity;
using EntityArchitect.CRUD.Queries;

namespace api2025.Queries.Definition;

public class GetPostCodeByCode() : Query<PostCode>("Queries/GetPostCodeByCode.sql", true);