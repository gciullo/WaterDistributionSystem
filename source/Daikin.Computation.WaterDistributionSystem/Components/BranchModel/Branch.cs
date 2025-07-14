using Daikin.Computation.WaterDistributionSystem.Components.UnitBlockModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.LinkModel;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Components.BranchModel;

public class Branch : Component, IDetectHandler<BranchInputData, BranchOutputData>
{
	#region Properties
	public double Resistance { get; set; }
	public int Id { get; set; }
	public UnitBlock UnitBlock { get; set; }
	public Guid ToId { get; set; }
	public Guid FromId { get; set; }
	public Guid RoomId { get; set; }
	public IReadOnlyCollection<LinkModel.Link> Links
	{ get; set; }
	public IReadOnlyCollection<LinkModel.Link> LinksInlet
	{ get; set; }

	public IReadOnlyCollection<LinkModel.Link> LinksOutlet
	{ get; set; }
	public IReadOnlyCollection<NodeModel.Node> Nodes
	{ get; set; }

	public IReadOnlyCollection<Guid> ComponentsId { get; set; }

	public Branch()
	{

	}
	protected override void RefreshComponents()
	{
		Links = null;
		UnitBlock = null;
		Nodes = null;
	}
	#endregion
	public async Task<BranchOutputData> Detect(BranchInputData input, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		BranchOutputData output = new BranchOutputData();
		output.OutputCalc = new List<Output>();

		#region LINK INLET
		//var linksInlet = Links.Where(x => x.ToId <= UnitBlock.FromId).ToList();
		foreach (var link in LinksInlet)
		{
			var results = await GetLinkOutputData(link, input, input.WaterInletTemperature, cancellationToken);
			output.OutputCalc.Add(results);

			output.PressureDrop += results.PressureDrop;
		}
		#endregion

		#region UNIT BLOCK
		var result = await GetUnitBlockData(UnitBlock, input, cancellationToken);
		output.OutputCalc.Add(result);
		output.ThermalCapacity = result.ThermalCapacity;
		output.WaterOutletTemperature = result.WaterOutletTemperature;
		output.UnitId = UnitBlock.Unit.Id;
		output.PressureDrop += result.PressureDrop;
		#endregion UNIT BLOCK

		#region LINK OUTLET
		//var linksOutlet = Links.Where(x => x.FromId >= UnitBlock.ToId).ToList();
		foreach (var link in LinksOutlet)
		{
			var results = await GetLinkOutputData(link, input, result.WaterOutletTemperature, cancellationToken);
			output.OutputCalc.Add(results);
			output.PressureDrop += result.PressureDrop;
		}
		#endregion
		output.Capacity = double.MinValue;
		foreach (var item in output.OutputCalc)
		{
			if (item.Capacity > output.Capacity) output.Capacity = item.Capacity;
		}
		output.Id = Id;
		output.RoomId = RoomId;

		return output;
	}
	internal (Guid FromId, Guid ToId) GetFromandToId()
	{
		List<Guid> MinId = new List<Guid>();
		List<Guid> MaxId = new List<Guid>();
		MinId.Add(Links.Select(x => x.FromId).Min());
		MaxId.Add(Links.Select(x => x.ToId).Max());

		MinId.Add(UnitBlock.FromId);
		MaxId.Add(UnitBlock.ToId);

		return (MinId.Min(), MaxId.Max());
	}

	public List<Guid> GetIdDistinctBranch()
	{
		List<Guid> output = new List<Guid>();
		foreach (var linkin in LinksInlet)
		{
			output.Add(linkin.FromId);
		}
		output.Add(UnitBlock.Unit.Id);
		foreach (var linkOut in LinksOutlet)
		{
			output.Add(linkOut.FromId);
		}
		return output;

	}
	internal double GetResistance()
	{
		double output = 0;
		var pipes = Links.Select(x => x.Pipe);
		foreach (var pipe in pipes)
		{
			output += pipe.GetResistance();
		}
		output += UnitBlock.GetResistance();
		return output;
	}
	private async Task<Output> GetLinkOutputData(LinkModel.Link links, BranchInputData input, double WaterInletTemp, CancellationToken cancellationToken)
	{
		LinkOutputData linkOutputData = new LinkOutputData();
		CalcData? inputLinkInlet = null;
		foreach (var inputCalc in input.InputCalc)
		{
			if (inputCalc.Id == links.FromId)
			{
				inputLinkInlet = inputCalc;
				break;
			}
		}

		LinkInputData linkInputData = new LinkInputData
		{
			WaterFlow = inputLinkInlet?.WaterFlow ?? 0
		};
		var result = await links.Detect(linkInputData, cancellationToken);

		Output output = new Output
		{
			Id = links.Pipe.Id,
			WaterFlow = linkInputData.WaterFlow,
			PressureDrop = result.PressureDrop,
			WaterOutletTemperature = WaterInletTemp
		};
		return output;

	}

	private async Task<Output> GetUnitBlockData(UnitBlock unitBlock, BranchInputData input, CancellationToken cancellationToken)
	{
		CalcData? inputUnit = null;
		foreach (var inputCalc in input.InputCalc)
		{
			if (inputCalc.Id == unitBlock.Unit.Id)
			{
				inputUnit = inputCalc;
				break;
			}
		}


		UnitBlockInputData unitBlockInputData = new UnitBlockInputData
		{
			WaterFlow = inputUnit?.WaterFlow ?? 0,
			FluidType = input.FluidType,
			Application = input.Application,
			WaterInletTemperature = input.WaterInletTemperature,
			GlycolPercentage = input.GlycolPercentage
		};
		var resultUnit = await UnitBlock.Detect(unitBlockInputData, cancellationToken);

		Output outputU = new Output
		{
			Id = UnitBlock.Unit.Id,
			Capacity = resultUnit.Capacity,
			WaterFlow = unitBlockInputData.WaterFlow,
			PressureDrop = resultUnit.PressureDrop,
			WaterOutletTemperature = resultUnit.WaterOutletTemperature,
			ThermalCapacity = resultUnit.ThermalCapacity
		};
		return outputU;
	}
}

