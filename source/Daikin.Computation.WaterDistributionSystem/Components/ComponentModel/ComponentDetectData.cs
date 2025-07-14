using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

public record ComponentInputData : BaseInputData, IDetectInput<ComponentOutputData>
{

}

public record ComponentOutputData : BaseOutputData
{

}

public class ComponentErrorCode : Enumeration
{
    private static int BaseId = 0;
    
    private ComponentErrorCode(int id, string name)
        : base(id, name)
    { }
}