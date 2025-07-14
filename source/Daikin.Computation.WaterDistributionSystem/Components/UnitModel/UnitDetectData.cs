
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Components.UnitModel;

public record UnitInputData : BaseInputData, IDetectInput<UnitOutputData>
{
    public double? PressureDrop { get; set; }
    public double? WaterFlow { get; set; }
    public double? Capacity { get; set; }
    public double WaterInletTemperature { get; set; }
    public double GlycolPercent { get; set; }
    public EnumData.FluidType FluidType { get; set; }
    public EnumData.Application Application { get; set; }
}

public record UnitOutputData : BaseOutputData
{
    public double SensibleCapacity { get; set; }  
    public double TotalCapacity { get; set; }  
    public double WaterFlow { get; set; }
    public double PressureDrop { get; set; }
    public double WaterOutletTemperature { get; set; }   
}

public class UnitErrorCode : Enumeration
{
    private static int BaseId = 1000;

    public static UnitErrorCode LoadTooLow = new UnitErrorCode(BaseId + 0, nameof(LoadTooLow));   

    private UnitErrorCode(int id, string name)
        : base(id, name)
    { }
}
