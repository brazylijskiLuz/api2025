using api2025.Enums;
using EntityArchitect.CRUD.Attributes.CrudAttributes;
using EntityArchitect.CRUD.Entities.Attributes;

namespace api2025.Entity;

[CannotCreate, CannotUpdate, CannotDelete, CannotGetById]
public class Report : EntityArchitect.CRUD.Entities.Entities.Entity
{
    public DateTime UsageTime { get; set; }
    public decimal ExpectedPension { get; set; }
    public Sex Sex { get; set; }
    public decimal SalaryAmount { get; set; }
    public bool ConsideredSickLeave { get; set; }
    public decimal AccountBalance { get; set; }
    public decimal SubAccountBalance { get; set; }
    public decimal Pension { get; set; }
    public decimal RealPension { get; set; }
    
    [OneToMany<PostCode>(nameof(PostCode.Reports))]
    public PostCode? PostalCode { get; set; } 
}