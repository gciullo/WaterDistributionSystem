using Daikin.Computation.WaterDistributionSystem.Components.LinkModel;
using Daikin.Computation.WaterDistributionSystem.Components.PipeModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class LinkComponentFromLinkEntityFactory : IComponentFactory<Components.LinkModel.Link>
{
    private readonly Link _entity;
    private readonly Components.PipeModel.Pipe _pipe;

    public LinkComponentFromLinkEntityFactory(Link entity, Components.PipeModel.Pipe pipe)
    {
        _entity = entity;
        _pipe = pipe;
    }

    public Components.LinkModel.Link BuildComponent()
    {       
        Components.LinkModel.Link component = new Components.LinkModel.Link()
        {
            FromId = _entity.FromId,
            ToId = _entity.ToId,
            //PipeId = _entity.PipeId,
            Pipe = _pipe
        };
        return component;
    }
}
