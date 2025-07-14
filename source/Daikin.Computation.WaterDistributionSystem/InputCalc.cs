using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Daikin.Computation.WaterDistributionSystem.DetectData.EnumData;

namespace Daikin.Computation.WaterDistributionSystem
{
    public class InputCalc
    {
        public List<Node> Nodes { get; set; }
        public List<Unit> Units { get; set; }
        public List<Pipe> Pipes { get; set; }
        public List<Link> Links { get; set; }
        public List<Room> Rooms { get; set; }
        //public List<double> DiametersAdmitted { get; init; } //Lista di diametri da cui pescare se il diametro di un tubo non è selezionato

        /// <summary>
        /// Restituisce il primo nodo comune alle unità in input, nella direzione suggerita dal booleano in input
        /// </summary>
        /// <param name="unitNodes"></param>
        /// <param name="isSupplyDirection"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Node GetCommonNode(IEnumerable<Unit> unitNodes, bool isSupplyDirection)
        {
            List<(Unit unit, IEnumerable<Guid> nodes)> UnitBlocks = new();

            foreach (var unit in unitNodes)
            {
                UnitBlocks.Add((unit, GetNodeList(unit.Id, isSupplyDirection)));
            }

            var distinctNodes = UnitBlocks.SelectMany(e => e.nodes).Distinct().ToList();

            var commonNodes = distinctNodes.Where(e => UnitBlocks.All(p => p.nodes.Contains(e)));

            var commonNode = Nodes.SingleOrDefault(n => n.Id == commonNodes.OrderBy(e => GetLayoutLevel(e, isSupplyDirection)).LastOrDefault());

            if (commonNode is null) throw new InvalidOperationException("No common node has been found.");

            return commonNode;
        }
        /// <summary>
        /// Restituisce la lista degli Id dei pipe che compongono il percorso dell'unità verso la direzione richiesta
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="isSupplyDirection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Guid> GetPipeUnitBlock(Unit unit, bool isSupplyDirection = true)
        {
            List<Guid> pipes = new();
            var pointedGuid = unit.Id;
            var chillerSideId = isSupplyDirection ? Guid.Parse("00000000-0000-0000-0000-000000000000") : Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            while (pointedGuid != chillerSideId)
            {
                var link = isSupplyDirection ? Links.SingleOrDefault(e => e.ToId == pointedGuid) : Links.SingleOrDefault(e => e.FromId == pointedGuid);

                if (link is null) throw new Exception($"Link to {pointedGuid} not found.");

                pointedGuid = isSupplyDirection ? link.FromId : link.ToId;

                pipes.Add(link.PipeId);
            }

            return pipes;
        }
        /// <summary>
        /// Restituisce la lista degli Id dei nodi che attraversano il percorso dell'oggetto verso la direzione richiesta
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="isSupplyDirection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal List<Guid> GetNodeList(Guid itemId, bool isSupplyDirection = true)
        {
            List<Guid> nodes = new();
            var pointedGuid = itemId;
            var chillerSideId = isSupplyDirection ? Guid.Parse("00000000-0000-0000-0000-000000000000") : Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            while (pointedGuid != chillerSideId)
            {
                var link = isSupplyDirection ? Links.SingleOrDefault(e => e.ToId == pointedGuid) : Links.SingleOrDefault(e => e.FromId == pointedGuid);

                if (link is null) throw new Exception($"Link to {pointedGuid} not found.");

                pointedGuid = isSupplyDirection ? link.FromId : link.ToId;
                nodes.Add(pointedGuid);
            }

            return nodes;
        }
        /// <summary>
        /// Restituisce il numero di nodi che mancano per raggiungere il Chiller Side nella direzione richiesta
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="isSupplyDirection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal int GetLayoutLevel(Guid nodeId, bool isSupplyDirection = true)
        {
            var pointedGuid = nodeId;
            var level = 0;
            var chillerSideId = isSupplyDirection ? Guid.Parse("00000000-0000-0000-0000-000000000000") : Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            while (pointedGuid != chillerSideId)
            {
                var link = isSupplyDirection ? Links.SingleOrDefault(e => e.ToId == pointedGuid) : Links.SingleOrDefault(e => e.FromId == pointedGuid);

                if (link is null) throw new Exception($"Link to {pointedGuid} not found.");

                pointedGuid = isSupplyDirection ? link.FromId : link.ToId;
                level++;
            }

            return level;
        }
    }
    public class Node
    {
        public Guid Id { get; set; }
    }
    public class Unit
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public List<PerformancePoint> PerformancePoints { get; init; } = new List<PerformancePoint>();
    }
    public class Room
    {
        public Guid Id { get; set; }
        public double Capacity { get; set; }        
        public double CapacityTolerance { get; set; }
    }
    public class Pipe
    {
        public Guid Id { get; set; }
        public double Length { get; set; }
        public double DeltaHeight { get; set; }
        public int NumberOfBends { get; set; }
        public double? Diameter { get; set; }
        public List<double> DiametersAdmitted { get; set; } = new();
        public PipeMaterial Material { get; set; }
    }
    public class Link
    {
        public Guid PipeId { get; set; }
        public Guid ToId { get; set; }
        public Guid FromId { get; set; }
    }
    public class PerformancePoint
    {
        public double Waterflow { get; set; }
        public double PressureDrop { get; set; }
        public double Capacity { get; set; }
        public double? CapacityTotal { get; set; }
        public double DeltaTemperature { get; set; }
    }
}
