
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Components.NodeModel;

public record RoomInputData : BaseInputData, IDetectInput<RoomOutputData>
{  
}

public record RoomOutputData : BaseOutputData
{  
}

public class RoomErrorCode : Enumeration
{
    private static int BaseId = 2200;
    public static RoomErrorCode LoadTooLow = new RoomErrorCode(BaseId + 0, nameof(LoadTooLow));
    public static RoomErrorCode MaxCurrentTooHigh = new RoomErrorCode(BaseId + 6, nameof(MaxCurrentTooHigh));

    private RoomErrorCode(int id, string name)
        : base(id, name)
    { }
}
