
using Daikin.Computation.WaterDistributionSystem.Components.BranchModel;
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;


namespace Daikin.Computation.WaterDistributionSystem.Components.SchemaModel;

public record SchemaInputData : IDetectInput<SchemaOutputData>
{
    public double WaterInletTemperature { get; set; }
    public double GlycolPercentage { get; set; }    
    public List<CalcData> Inputs { get; set; }
    public EnumData.FluidType FluidType { get; set; }
    public EnumData.Application Application { get; set; }
}

public record SchemaOutputData : BaseOutputData
{
    public double Capacity { get; set; }
    public double WaterOutletTemperature { get; set; }
    public List<BranchOutputData> BranchesOutput { get; set; }
    public List<double> DeltaCapacity = new();
    public List<double> DeltaPressureDrop = new();
}



public class SchemaErrorCode : Enumeration
{
  
    private SchemaErrorCode(int id, string name)
        : base(id, name)
    { }
}
