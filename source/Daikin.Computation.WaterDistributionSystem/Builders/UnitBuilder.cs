using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Builders;

public class UnitBuilder : ComponentBuilder<Components.UnitModel.Unit>
{
    public UnitBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
    }
    public UnitBuilder WithName(string name)
    {
        Component.Name = name;
        return this;
    }
}