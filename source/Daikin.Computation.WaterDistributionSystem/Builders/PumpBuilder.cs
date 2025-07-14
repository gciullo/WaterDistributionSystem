using Daikin.Computation.WaterDistributionSystem.Components.PumpModel;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Builders;

public class PumpBuilder : ComponentBuilder<Pump>
{
    public PumpBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {       
    }
    public PumpBuilder WithName(string name)
    {
        Component.Name = name;
        return this;
    }
}

