
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;


namespace Daikin.Computation.WaterDistributionSystem.Components.BranchModel;

public record BranchInputData : IDetectInput<BranchOutputData>
{  
   
    public double WaterInletTemperature { get; set; }
    public double GlycolPercentage { get; set; }
    public List<CalcData> InputCalc { get; set; }   
    public EnumData.FluidType FluidType { get; set; }
    public EnumData.Application Application { get; set; }
}

public record BranchOutputData : BaseOutputData
{
    public int Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UnitId { get; set; }
    public double Capacity { get; set; }
    public double PressureDrop { get; set; }
    public double ThermalCapacity { get; set; } 
    public double WaterOutletTemperature { get;set; }
    public List<Output> OutputCalc { get; set; }
}

public class BranchErrorCode : Enumeration
{
    private BranchErrorCode(int id, string name)
        : base(id, name)
    { }
}

