using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;


namespace Daikin.Computation.WaterDistributionSystem.Components.PipeModel;

public record PipeInputData : BaseInputData, IDetectInput<PipeOutputData>
{    public double WaterFlow { get; set; }    
}

public record PipeOutputData : BaseOutputData
{    public double PressureDrop { get; set; }
}

public class TubeErrorCode : Enumeration
{
    private static int BaseId = 2100;
    public static TubeErrorCode LoadTooLow = new TubeErrorCode(BaseId + 0, nameof(LoadTooLow));
    public static TubeErrorCode MaxCurrentTooHigh = new TubeErrorCode(BaseId + 6, nameof(MaxCurrentTooHigh));

    private TubeErrorCode(int id, string name)
        : base(id, name)
    { }
}