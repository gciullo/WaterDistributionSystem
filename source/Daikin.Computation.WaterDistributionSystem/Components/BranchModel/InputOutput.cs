
namespace Daikin.Computation.WaterDistributionSystem.Components.BranchModel;


public class Output : CalcData
{
    public double PressureDrop { get; set; }
    public double Capacity { get; set; }
    public double WaterOutletTemperature { get; set; }

    public double ThermalCapacity { get; set; }
    public Output()
    {
        
    }
}

