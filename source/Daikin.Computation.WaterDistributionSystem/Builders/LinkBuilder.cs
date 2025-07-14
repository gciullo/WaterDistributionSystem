using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Builders;

public class LinkBuilder : ComponentBuilder<Daikin.Computation.WaterDistributionSystem.Components.LinkModel.Link>
{
    public LinkBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
    }    public LinkBuilder WithName(string name)
    {        
        return this;
    }
}