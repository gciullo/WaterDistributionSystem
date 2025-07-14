
using Daikin.Computation.WaterDistributionSystem.Components.BranchModel;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Components.SchemaModel;

public class Schema : Component, IDetectHandler<SchemaInputData, SchemaOutputData>
{
	#region Properties
	private IReadOnlyCollection<BranchModel.Branch>? _branches;
	public IReadOnlyCollection<BranchModel.Branch> Branches
	{
		get
		{
			if (_branches is null)
			{
				_branches = Components.OfType<BranchModel.Branch>().ToList().AsReadOnly();
			}
			return _branches;
		}
	}
	private IReadOnlyCollection<UnitBlockModel.UnitBlock>? _unitBlocks;
	public IReadOnlyCollection<UnitBlockModel.UnitBlock> UnitBlocks
	{
		get
		{
			if (_unitBlocks is null)
			{
				_unitBlocks = Components.OfType<UnitBlockModel.UnitBlock>().ToList().AsReadOnly();
			}
			return _unitBlocks;
		}
	}
	private IReadOnlyCollection<LinkModel.Link>? _links;
	public IReadOnlyCollection<LinkModel.Link> Links
	{
		get
		{
			if (_links is null)
			{
				_links = Components.OfType<LinkModel.Link>().ToList().AsReadOnly();
			}
			return _links;
		}
	}
	private IReadOnlyCollection<NodeModel.Node>? _nodes;
	public IReadOnlyCollection<NodeModel.Node> Nodes
	{
		get
		{
			if (_nodes is null)
			{
				_nodes = Components.OfType<NodeModel.Node>().ToList().AsReadOnly();
			}
			return _nodes;
		}
	}
	private IReadOnlyCollection<RoomModel.Room>? _rooms;
	public IReadOnlyCollection<RoomModel.Room> Rooms
	{
		get
		{
			if (_rooms is null)
			{
				_rooms = Components.OfType<RoomModel.Room>().ToList().AsReadOnly();
			}
			return _rooms;
		}
	}
	private IReadOnlyCollection<PipeModel.Pipe>? _pipes;
	public IReadOnlyCollection<PipeModel.Pipe> Pipes
	{
		get
		{
			if (_pipes is null)
			{
				_pipes = Components.OfType<PipeModel.Pipe>().ToList().AsReadOnly();
			}
			return _pipes;
		}
	}
	public List<LinkModel.Link> LinksInlet { get; set; }
	public List<LinkModel.Link> LinkOutlet { get; set; }
	public Schema()
	{
	}
	#endregion
	protected override void RefreshComponents()
	{
		_branches = null;
		_rooms = null;
		_unitBlocks = null;
		_links = null;
		_nodes = null;
		_pipes = null;
	}

	#region Public Methods
	public async Task<SchemaOutputData> Detect(SchemaInputData inputData, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		SchemaOutputData output = new SchemaOutputData();
		output.BranchesOutput = new List<BranchOutputData>();
		try
		{
			var branchesOutput = await GetBranchesOutput(inputData, cancellationToken);
			output.BranchesOutput = branchesOutput;
			output.Capacity = 0;
			foreach (var branchOutput in branchesOutput) output.Capacity += branchOutput.Capacity;
			output.Capacity = Math.Round(output.Capacity, 2);
			output.WaterOutletTemperature = inputData.WaterInletTemperature;
			if (output.Capacity > 0)
			{
				output.DeltaPressureDrop = CheckPressureDrop(branchesOutput);
				output.DeltaCapacity = CheckCapacity(branchesOutput);
				double thermals = 0;
				double flows = 0;

				foreach(var branchOutput in branchesOutput)
				{
					var two = branchOutput.WaterOutletTemperature + 273.15;
					thermals += branchOutput.ThermalCapacity;
					flows += branchOutput.ThermalCapacity / two;
                }

				output.WaterOutletTemperature = Math.Round((thermals / flows) - 273.15, 2);
			}

		}
		catch (Exception ex)
		{

		}


		return output;
	}
	#endregion

	#region Architectural Methods
	/// <summary>
	/// Restituisce il primo nodo comune alle unità in input, nella direzione suggerita dal booleano in input
	/// </summary>
	/// <param name="unitNodes"></param>
	/// <param name="isSupplyDirection"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	internal NodeModel.Node GetCommonNode(IEnumerable<UnitModel.Unit> unitNodes, bool isSupplyDirection)
	{
		List<(UnitModel.Unit unit, IEnumerable<Guid> nodes)> UnitBlocks = new();

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
	internal List<Guid> GetPipeUnitBlock(UnitModel.Unit unit, bool isSupplyDirection = true)
	{
		List<Guid> pipes = new();
		var pointedGuid = unit.Id;
		var chillerSideId = isSupplyDirection ? Guid.Parse("00000000-0000-0000-0000-000000000000") : Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

		while (pointedGuid != chillerSideId)
		{
			var link = isSupplyDirection ? Links.SingleOrDefault(e => e.ToId == pointedGuid) : Links.SingleOrDefault(e => e.FromId == pointedGuid);

			if (link is null) throw new Exception($"Link to {pointedGuid} not found.");

			pointedGuid = isSupplyDirection ? link.FromId : link.ToId;

			pipes.Add(link.Pipe.Id);
		}
		return pipes;
	}
	/// <summary>
	/// Restituisce la lista dei link che corrispondono agli Id dei pipe che compongono il percorso dell'unità verso la direzione richiesta
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="isSupplyDirection"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	internal List<LinkModel.Link> GetLinksUnitBlock(List<Guid> id)
	{
		List<LinkModel.Link> links = new();
		foreach (var idItem in id)
		{
			foreach (var link in Links)
			{
				if (link.Pipe.Id == idItem) links.Add(link);
			}
		}
		return links;
	}
	/// <summary>
	/// Restituisce il nodo più vicino seguendo il percorso dell'oggetto verso la direzione richiesta
	/// </summary>
	/// <param name="itemId"></param>
	/// <param name="isSupplyDirection"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	internal NodeModel.Node SearchNearestNode(Guid itemId, bool isSupplyDirection = true)
	{
		var result = GetNodeList(itemId, isSupplyDirection);
		var nodes = result.FirstOrDefault();
		return Nodes.Where(x => x.Id == nodes).Single();
	}
	internal List<LinkModel.Link> GetLinksBetweenNodes(Guid EndId, Guid StartId, List<LinkModel.Link> LinkUnit, bool isSupplyDirection = true)
	{
		List<LinkModel.Link> links = new();
		var pointedGuid = EndId;
		var chillerSideId = isSupplyDirection ? StartId : EndId;

		while (pointedGuid != chillerSideId)
		{

			var link = isSupplyDirection ? LinkUnit.SingleOrDefault(e => e.ToId == pointedGuid) : LinkUnit.SingleOrDefault(e => e.FromId == pointedGuid);

			if (link is null) throw new Exception($"Link to {pointedGuid} not found.");

			pointedGuid = isSupplyDirection ? link.FromId : link.ToId;

			links.Add(link);
		}
		var output = links.OrderBy(x => x.FromId).ToList();
		return output;
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
			LinkModel.Link? link = null;
			foreach(var item in Links)
			{
				if (isSupplyDirection && item.ToId == pointedGuid)
				{
					link = item; break;
				}
				else if (!isSupplyDirection && item.FromId == pointedGuid)
				{
					link = item; break;
				}
			}
			if (link is null) throw new Exception($"Link to {pointedGuid} not found.");

			pointedGuid = isSupplyDirection ? link.FromId : link.ToId;
			nodes.Add(pointedGuid);
		}

		return nodes;
	}
	/// <summary>
	/// Restituisce la lista dei Nodi che appartengono al UnitBlock verso la direzione richiesta
	/// </summary>
	/// <param name="itemId"></param>
	/// <param name="isSupplyDirection"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	internal List<NodeModel.Node> GetNodesFromId(List<Guid> id)
	{
		List<NodeModel.Node> nodes = new();
		foreach (var idItem in id)
		{
			foreach (var node in Nodes)
			{
				if (node.Id == idItem) nodes.Add(node);
			}
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
	#endregion

	#region CheckConstraints
	private List<double> CheckCapacity(List<BranchOutputData> branchOutputDatas)
	{
		List<double> delta = new();
		if (branchOutputDatas.Count() == 0) return delta;

		List<Guid> rooms = new();
		foreach (var branchOutputData in branchOutputDatas) 
			if (!rooms.Contains(branchOutputData.RoomId)) rooms.Add(branchOutputData.RoomId);

		foreach (var room in rooms)
		{
			double Cap = 0;
			foreach (var branchOutputData in branchOutputDatas)
			{
				if (branchOutputData.RoomId == room) Cap += branchOutputData.Capacity;
			}
			double CapReq = 0;
			double toll = 0;
			foreach (var capRoom in Rooms)
			{
				if (capRoom.Id == room) CapReq += capRoom.Capacity;
			}
			foreach (var capToll in Rooms)
			{
				if (capToll.Id == room)
				{
					toll = capToll.CapacityTolerance;
					break;
				}
			}
			var deltaCap = Math.Abs((CapReq - Cap)) / CapReq * 100;
			if (deltaCap <= toll) deltaCap = toll;
			delta.Add(Math.Abs(deltaCap - toll));

		}
		return delta;

	}
	private List<double> CheckPressureDrop(List<BranchOutputData> branchOutputDatas)
	{
		List<double> delta = new List<double>();
		if (branchOutputDatas.Count() == 0) return delta;
		double pressmin = Double.MaxValue;
		foreach(var branchOutputData in branchOutputDatas)
		{
			if (branchOutputData.PressureDrop > 0 && pressmin > branchOutputData.PressureDrop) pressmin = branchOutputData.PressureDrop;
		}
		foreach (var result in branchOutputDatas)
		{
			var x = result.PressureDrop - pressmin;
			if (x < pressmin * 0.05) x = 0;
			delta.Add(x);
		}
		return delta;
	}
	public List<CalcData> GetEmptyCalcData()
	{
		List<CalcData> data = new List<CalcData>();

		List<Guid> idInputs = new List<Guid>();
		//var branches = Branches.OrderBy(e => e.Id).ToList();
		//var linksin = Branches.SelectMany(e => e.LinksInlet.Select(x => x.FromId)).Distinct().ToList();
		//idInputs.AddRange(linksin);
		//idInputs.AddRange(Branches.Select(e => e.UnitBlock.Unit.Id).Where(e => !idInputs.Contains(e)).Distinct().ToList());
		//var linksOut = Branches.SelectMany(e => e.LinksOutlet.Select(x => x.FromId)).Distinct().ToList();
		//idInputs.AddRange(linksOut);
		idInputs = UnitBlocks.Select(u => u.Unit.Id).ToList();

		foreach (var input in idInputs)
		{
			CalcData calcData = new CalcData()
			{
				Id = input,
				WaterFlow = 0
			};
			data.Add(calcData);
		}
		return data;
	}
	private async Task<List<BranchOutputData>> GetBranchesOutput(SchemaInputData inputData, CancellationToken cancellationToken)
	{
		//var branches = Branches.OrderBy(e => e.Id).ToList();
		List<BranchOutputData> output = new();
		foreach (var branch in Branches)
		{
			List<Guid> idInputs = branch.GetIdDistinctBranch();

			List<CalcData> inputsBranches = new();

			foreach(var input in inputData.Inputs)
			{
				if (idInputs.Contains(input.Id)) inputsBranches.Add(input);
			}

			BranchInputData branchInputData = new BranchInputData()
			{
				FluidType = inputData.FluidType,
				Application = inputData.Application,
				GlycolPercentage = inputData.GlycolPercentage,
				WaterInletTemperature = inputData.WaterInletTemperature,
				InputCalc = inputsBranches

			};
			var branchOutput = await branch.Detect(branchInputData, cancellationToken);

			output.Add(branchOutput);
		}
		return output;
	}
	#endregion

	#region Routes Evaluation
	internal List<LinkModel.Link> FindInletLinks(IReadOnlyCollection<LinkModel.Link> allLinks, IReadOnlyCollection<UnitBlockModel.UnitBlock> allUnitBlocks)
	{
		// Find links where ToId <= any UnitBlock's FromId
		var matchingLinks = allLinks
			.Where(link => allUnitBlocks.Any(unitBlock => link.ToId.CompareTo(unitBlock.FromId) <= 0))
			.ToList();

		return matchingLinks;
	}
	internal List<LinkModel.Link> FindOutletLinks(IReadOnlyCollection<LinkModel.Link> allLinks, IReadOnlyCollection<UnitBlockModel.UnitBlock> allUnitBlocks)
	{
		// Find links where FromId >= any UnitBlock's ToId
		var matchingLinks = allLinks
			.Where(link => allUnitBlocks.Any(unitBlock => link.FromId.CompareTo(unitBlock.ToId) >= 0))
			.ToList();

		return matchingLinks;
	}
	#endregion
}
