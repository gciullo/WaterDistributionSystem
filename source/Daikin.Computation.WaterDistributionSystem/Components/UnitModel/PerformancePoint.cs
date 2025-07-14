namespace Daikin.Computation.WaterDistributionSystem.Components.UnitModel;

public class PerformancePoint
{
    public PerformancePoint(double waterFlow, double pressureDrop, double capacity, double? capacityTotal, double deltaTemperature)
    {
        Waterflow = waterFlow;
        PressureDrop = pressureDrop;
        SensibleCapacity = capacity;
        TotalCapacity = capacityTotal ?? capacity;
        DeltaTemperature = deltaTemperature;
    }
    public double Waterflow { get; set; }
    public double PressureDrop { get; set; }
    public double SensibleCapacity { get; set; }
    public double TotalCapacity { get; set; }
    public double DeltaTemperature { get; set; }
}
