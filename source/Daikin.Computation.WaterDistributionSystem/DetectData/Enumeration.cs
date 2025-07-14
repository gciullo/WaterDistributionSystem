namespace Daikin.Computation.WaterDistributionSystem.DetectData;

public class Enumeration
{

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public override string ToString() => Name;
    public override int GetHashCode() => Id;
}
