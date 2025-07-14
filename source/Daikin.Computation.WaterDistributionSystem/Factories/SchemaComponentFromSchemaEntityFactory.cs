using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.SchemaModel;
using Daikin.Computation.WaterDistributionSystem.Components.UnitBlockModel;
using Daikin.Computation.WaterDistributionSystem.Components.BranchModel;

namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class SchemaComponentFromSchemaEntityFactory : Component, IComponentFactory<Schema>
{
    private readonly InputCalc _entity;
    public SchemaComponentFromSchemaEntityFactory(InputCalc entityData)
    {     
        _entity = entityData;
    }
    public Schema BuildComponent()
    {
        Schema component = new Schema();

        foreach (var node in _entity.Nodes)
        {
            IComponentFactory<Components.NodeModel.Node> nodeFactory = new NodeComponentFromNodeEntityFactory(node);
            var p = nodeFactory.BuildComponent();
            component.AddComponent(p);
        }
        foreach (var pipe in _entity.Pipes)
        {
            IComponentFactory<Components.PipeModel.Pipe> pipeFactory = new PipeComponentFromPipeEntityFactory(pipe);
            var p = pipeFactory.BuildComponent();
            component.AddComponent(p);
        }
        foreach (var link in _entity.Links)
        {
            var pipe = component.Pipes.Where(x => x.Id == link.PipeId).SingleOrDefault();

            IComponentFactory<Components.LinkModel.Link> linkFactory = new LinkComponentFromLinkEntityFactory(link, pipe);
            var l = linkFactory.BuildComponent();
            component.AddComponent(l);
        }
        foreach (var room in _entity.Rooms)
        {
            IComponentFactory<Components.RoomModel.Room> roomFactory = new RoomComponentFromRoomEntityFactory(room);
            var p = roomFactory.BuildComponent();
            component.AddComponent(p);
        }
        List<Components.LinkModel.Link> linksInlet = new();
        List<Components.LinkModel.Link> linksOutlet = new();
        List<Components.LinkModel.Link> linksUnitBlock = new();
        int id = 0;
        foreach (var unit in _entity.Units)
        {
            IComponentFactory<Components.UnitModel.Unit> unitFactory = new UnitComponentFromUnitEntityFactory(unit);
            var unitUnitBlock = unitFactory.BuildComponent();
            component.AddComponent(unitUnitBlock);

            linksInlet = component.GetLinksUnitBlock(component.GetPipeUnitBlock(unitUnitBlock, true));
            var LinkIn = linksInlet.OrderBy(x=>x.FromId).ToList();
            linksOutlet = component.GetLinksUnitBlock(component.GetPipeUnitBlock(unitUnitBlock, false));

            List<Components.NodeModel.Node> Nodes = new List<Components.NodeModel.Node>
            {
                component.SearchNearestNode(unitUnitBlock.Id, true),
                component.SearchNearestNode(unitUnitBlock.Id, false)
            };

            var LinkTotal = LinkIn.Union(linksOutlet).ToList();
            var LinksInBlock = linksInlet.Where(x => Nodes.Any(n => n.Id.Equals(x.FromId))).ToList();
            var LinksOutBlock = linksOutlet.Where(x => Nodes.Any(n => n.Id.Equals(x.ToId))).ToList();      
            linksUnitBlock = LinksInBlock.Union(LinksOutBlock).ToList();


            IComponentFactory<UnitBlock> UnitBlockFactory = new UnitBlockComponentFromUnitBlockEntityFactory(unitUnitBlock, linksUnitBlock, Nodes);
            var UnitBlockSchema = UnitBlockFactory.BuildComponent();
            UnitBlockSchema.Id = id;
            component.AddComponent(UnitBlockSchema);

            component.LinksInlet = LinkIn.Except(linksUnitBlock).ToList();
            component.LinkOutlet = linksOutlet.Except(linksUnitBlock).ToList();

            //var linksBranch = component.LinksInlet.Union(component.LinkOutlet).ToList();

            IComponentFactory<Branch> BranchFactory = new BranchComponentFromBranchEntityFactory(UnitBlockSchema, component.LinksInlet,component.LinkOutlet, Nodes);
            var p =  BranchFactory.BuildComponent();
            p.Id = id;
            component.AddComponent(p);
            id++;

        }

        return component;
    }

    

    
}
