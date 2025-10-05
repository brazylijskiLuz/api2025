using EntityArchitect.CRUD.Attributes.CrudAttributes;
using EntityArchitect.CRUD.Entities.Attributes;

namespace api2025.Entity;

[CannotCreate, CannotUpdate, CannotDelete, CannotGetById]
public class PostCode : EntityArchitect.CRUD.Entities.Entities.Entity
{
    public string Code { get; set; }
    
    [OneToMany<Province>(nameof(Province.PostCodes))]
    public Province Province { get; set; }
    
    [ManyToOne<Report>(nameof(Report.PostalCode))]
    public List<Report> Reports { get; set; }
}