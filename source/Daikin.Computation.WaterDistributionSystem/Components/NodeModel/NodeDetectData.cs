
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Components.NodeModel;

public record NodeInputData : BaseInputData, IDetectInput<NodeOutputData>
{  
}

public record NodeOutputData : BaseOutputData
{  
}

public class NodeErrorCode : Enumeration
{
    private static int BaseId = 2200;
    public static NodeErrorCode LoadTooLow = new NodeErrorCode(BaseId + 0, nameof(LoadTooLow));
    public static NodeErrorCode MaxCurrentTooHigh = new NodeErrorCode(BaseId + 6, nameof(MaxCurrentTooHigh));

    private NodeErrorCode(int id, string name)
        : base(id, name)
    { }
}
