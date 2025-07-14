using Daikin.Computation.WaterDistributionSystem.Components;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.UnitBlockModel;

namespace Daikin.Computation.WaterDistributionSystem.Builders;

public class UnitBlockBuilder : ComponentBuilder<UnitBlock>
{
    public UnitBlockBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
    }
    public UnitBlockBuilder WithName(string name)
    {
        Component.Name = name;
        return this;
    }
}