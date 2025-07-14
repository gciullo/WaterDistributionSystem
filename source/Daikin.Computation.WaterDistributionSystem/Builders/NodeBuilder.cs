using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Builders;

public class NodeBuilder : ComponentBuilder<Components.NodeModel.Node>
{
    public NodeBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
    }
    public NodeBuilder WithName(string name)
    {
        return this;
    }  
}