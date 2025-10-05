using EntityArchitect.CRUD.Attributes.CrudAttributes;
using EntityArchitect.CRUD.Entities.Attributes;

namespace api2025.Entity;
[CannotCreate, CannotUpdate, CannotDelete, CannotGetById]
public class Province : EntityArchitect.CRUD.Entities.Entities.Entity
{
    public string Name { get; set; }
    
    [ManyToOne<PostCode>(nameof(PostCode.Province))]
    public List<PostCode> PostCodes { get; set; }
}