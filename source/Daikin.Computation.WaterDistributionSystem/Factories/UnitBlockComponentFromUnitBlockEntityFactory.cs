using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.UnitBlockModel;

namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class UnitBlockComponentFromUnitBlockEntityFactory : Component, IComponentFactory<UnitBlock>
{
    private readonly Components.UnitModel.Unit _entityUnit;
    private readonly List<Components.LinkModel.Link> _entityLinks;
    private readonly List<Components.NodeModel.Node> _entityNodes;
    public UnitBlockComponentFromUnitBlockEntityFactory(Components.UnitModel.Unit entityUnit, 
        List<Components.LinkModel.Link> entityLinks, List<Components.NodeModel.Node> entityNodes)
    {     
        _entityUnit = entityUnit;
        _entityLinks = entityLinks;
        _entityNodes = entityNodes;
    }
    public UnitBlock BuildComponent()
    {
        UnitBlock component = new UnitBlock();
     
        component.Unit = _entityUnit;
        component.Links = _entityLinks;
        component.Nodes = _entityNodes;

        component.Resistance = component.GetResistance();
        var nodeId = component.GetFromandToId();
        component.FromId = nodeId.FromId;
        component.ToId = nodeId.ToId;
        //component.UniId = component.Unit.Id;
        return component;
    }
}
