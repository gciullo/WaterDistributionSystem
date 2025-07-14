using Daikin.Computation.WaterDistributionSystem.Components.NodeModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using System.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class NodeComponentFromNodeEntityFactory : Component, IComponentFactory<Components.NodeModel.Node>
{
    private readonly Node _entity;

    public NodeComponentFromNodeEntityFactory(Node entity)
    {
        _entity = entity;
    }

    public Components.NodeModel.Node BuildComponent()
    {
        Components.NodeModel.Node component = new Components.NodeModel.Node()
        {
            Id = _entity.Id           
        };
        return component;
    }
}
