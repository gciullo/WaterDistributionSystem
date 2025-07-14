using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.UnitBlockModel;
using Daikin.Computation.WaterDistributionSystem.Components.BranchModel;


namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class BranchComponentFromBranchEntityFactory : Component, IComponentFactory<Components.BranchModel.Branch>
{
    private readonly Components.UnitBlockModel.UnitBlock _entityUnit;
    private readonly List<Components.LinkModel.Link> _entityLinksInlet;
    private readonly List<Components.LinkModel.Link> _entityLinksOutlet;
    private readonly List<Components.NodeModel.Node> _entityNodes;
    public BranchComponentFromBranchEntityFactory(UnitBlock entityUnit, 
        List<Components.LinkModel.Link> entityLinksInlet, List<Components.LinkModel.Link> entityLinksOutlet, List<Components.NodeModel.Node> entityNodes)
    {     
        _entityUnit = entityUnit;
        _entityLinksInlet = entityLinksInlet;
        _entityLinksOutlet = entityLinksOutlet;
        _entityNodes = entityNodes;
    }
    public Branch BuildComponent()
    {
        Branch component = new Branch();

        component.UnitBlock = _entityUnit;
        component.LinksInlet = _entityLinksInlet;
        component.LinksOutlet = _entityLinksOutlet;
        component.Links = component.LinksInlet.Union(component.LinksOutlet).ToList();
        component.Nodes = _entityNodes;
        component.Resistance = component.GetResistance();  
        component.FromId = component.Links.Select(x => x.FromId).FirstOrDefault();
        component.ToId = component.Links.Select(x => x.ToId).LastOrDefault();
        component.RoomId = component.UnitBlock.Unit.RoomId;

        return component;
    }
}
