using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
namespace Daikin.Computation.WaterDistributionSystem.Builders;

public class SchemaBuilder : ComponentBuilder<Components.SchemaModel.Schema>
{
    public SchemaBuilder(ComponentBuilder parentComponentBuilder, Component parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
    }
    public SchemaBuilder WithName(string name)
    {
        Component.Name = name;
        return this;
    }   
}