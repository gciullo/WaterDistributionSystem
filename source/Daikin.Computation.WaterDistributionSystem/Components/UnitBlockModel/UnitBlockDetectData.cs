
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
namespace Daikin.Computation.WaterDistributionSystem.Components.UnitBlockModel;

public record UnitBlockInputData : IDetectInput<UnitBlockOutputData>
{    
    public double WaterFlow { get; set; }
    public double WaterInletTemperature { get; set; }
    public double GlycolPercentage { get; set; }
    public EnumData.FluidType FluidType { get; set; }
    public EnumData.Application Application { get; set; }
}

public record UnitBlockOutputData : BaseOutputData
{
    public double Capacity { get; set; }
    public double WaterOutletTemperature { get; set; }
    public double PressureDrop { get; set; }
    public double WaterFlow { get; set; }
    public double ThermalCapacity { get; set; }



}

public class UnitBlockErrorCode : Enumeration
{
  
    private UnitBlockErrorCode(int id, string name)
        : base(id, name)
    { }
}
