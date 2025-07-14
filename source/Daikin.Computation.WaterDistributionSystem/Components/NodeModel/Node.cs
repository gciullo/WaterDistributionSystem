
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Components.NodeModel;

public class Node : Component
{
    public Guid Id { get; set; }
    public  void Detect()
    {
        throw new NotImplementedException();
    }
}
