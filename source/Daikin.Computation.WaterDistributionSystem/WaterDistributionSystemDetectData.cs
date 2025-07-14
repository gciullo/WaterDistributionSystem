
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem;

namespace Daikin.Computation.WaterDistributionSystem.Components.SchemaModel;

public record WaterDistributionSystemInputData : IDetectInput<WaterDistributionSystemOutputData>
{
    public double WaterInletTemperature { get; set; }   
    public double GlycolPercentage { get; set; }
    public List<CalcData> InputCalc { get; set; }
    public EnumData.FluidType FluidType { get; set; }
    public EnumData.Application Application { get; set; }
    public bool AllowSwitchOffUnit { get; set; }
}

public record WaterDistributionSystemOutputData : BaseOutputData
{
    public double Capacity { get; set; }
    public double WaterOutletTemperature { get; set; }
    public SchemaOutputData SchemaOutputs { get; set; }
    public List<double> DeltaCapacity { get; set; }
    public List<double> DeltaPressureDrop { get; set; }

    public double PressureDrop { get; set; }    

    
}

public class WaterDistributionSystemErrorCode : Enumeration
{
  
    private WaterDistributionSystemErrorCode(int id, string name)
        : base(id, name)
    { }
}
