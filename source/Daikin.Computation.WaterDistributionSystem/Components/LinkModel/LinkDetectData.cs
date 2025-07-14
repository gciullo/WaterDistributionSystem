using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;


namespace Daikin.Computation.WaterDistributionSystem.Components.LinkModel;

public record LinkInputData : BaseInputData, IDetectInput<LinkOutputData>
{  
    public double WaterFlow { get; set; }
}

public record LinkOutputData : BaseOutputData
{
    public double WaterFlow { get; set; }
    public double PressureDrop { get; set;}
}

public class LinkErrorCode : Enumeration
{
    private static int BaseId = 2200;
    public static LinkErrorCode LoadTooLow = new LinkErrorCode(BaseId + 0, nameof(LoadTooLow));
    public static LinkErrorCode MaxCurrentTooHigh = new LinkErrorCode(BaseId + 6, nameof(MaxCurrentTooHigh));

    private LinkErrorCode(int id, string name)
        : base(id, name)
    { }
}
