using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class PipeComponentFromPipeEntityFactory : IComponentFactory<Components.PipeModel.Pipe>
{
    private readonly Pipe _entity;

    public PipeComponentFromPipeEntityFactory(Pipe entity)
    {
        _entity = entity;
    }
    public Components.PipeModel.Pipe BuildComponent()
    {
        Components.PipeModel.Pipe component = new Components.PipeModel.Pipe()
        {
            Id = _entity.Id,
            DeltaHeight = _entity.DeltaHeight,
            Diameter = _entity.Diameter,
            Length = _entity.Length,
            NumberOfBends = _entity.NumberOfBends
        };
        component.Resistance = component.GetResistance();
        return component;
    }
}
