namespace Daikin.Computation.WaterDistributionSystem;

public class CalcData : ICloneable
{
    public double WaterFlow { get; set; }
    public Guid Id { get; set; }
    public CalcData()
    {
    }
    public object Clone()
    {
        return new CalcData
        {
            WaterFlow = this.WaterFlow,
            Id = this.Id
        };
    }
}

