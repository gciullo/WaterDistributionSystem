using Daikin.Computation.WaterDistributionSystem.Components.PipeModel;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Builders;
public class PipeBuilder : ComponentBuilder<Components.PipeModel.Pipe>
{
    public PipeBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
    }
    public PipeBuilder WithName(string name)
    {
        Component.Name = name;
        return this;
    }
}