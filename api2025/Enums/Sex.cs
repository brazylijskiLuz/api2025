using EntityArchitect.CRUD.Enumerations;

namespace api2025.Enums;

public class Sex : Enumeration
{
    public static Sex Male = new Sex(1, "Męszczyzna");
    public static Sex Female = new Sex(1, "Kobieta");

    public Sex(int id) : base(id)
    {
    }

    public Sex(int id, string name) : base(id, name)
    {
    }
}