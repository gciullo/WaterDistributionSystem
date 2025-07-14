using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Builders;

public class RoomBuilder : ComponentBuilder<Components.RoomModel.Room>
{
    public RoomBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
    }
    public RoomBuilder WithName(string name)
    {
        return this;
    }  
}