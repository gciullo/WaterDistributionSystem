using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.GAModel;
using Daikin.Computation.WaterDistributionSystem.Components.SchemaModel;
using Daikin.Computation.WaterDistributionSystem.Factories;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem;

public class WaterDistributionSystem : Component, IDetectHandler<WaterDistributionSystemInputData, WaterDistributionSystemOutputData>
{
	public Schema Schema { get; set; }
	public GeneticAlgorithm GeneticAlgorithm { get; set; }
	public double NominalFlow { get; set; }
	public double Minflow { get; set; }
	public double Maxflow { get; set; }

	public WaterDistributionSystem()
	{
	}
	public async Task<WaterDistributionSystemOutputData> Detect(WaterDistributionSystemInputData input, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		GAInputData inputGA = new GAInputData()
		{
			waterDistributionSystemInput = input,
			nominalValue = NominalFlow,
			maxValue = Maxflow,
			minValue = Minflow,
			populationSize = Math.Min(25 * Schema.Branches.Count(), 50),
			generations = Math.Min(Schema.Branches.Count() * 250, 500),
			mutationRate = 5
		};
		GeneticAlgorithm.WaterDistributionSystem = this;

		try
		{
			var res = await GeneticAlgorithm.Detect(inputGA, cancellationToken);
		}
		catch (Exception e)
		{
		}

		SchemaInputData schemaInputData = new SchemaInputData()
		{
			FluidType = input.FluidType,
			Application = input.Application,
			WaterInletTemperature = input.WaterInletTemperature,
			GlycolPercentage = input.GlycolPercentage,
			Inputs = GeneticAlgorithm.CreateInputSchema(GeneticAlgorithm.Solution)
		};
		WaterDistributionSystemOutputData output = new WaterDistributionSystemOutputData();
		try
		{
			var result = await Schema.Detect(schemaInputData, cancellationToken);
			output.Capacity = result.Capacity;
			output.DeltaPressureDrop = result.DeltaPressureDrop;
			output.DeltaCapacity = result.DeltaCapacity;
			output.PressureDrop = result.BranchesOutput.Select(x => x.PressureDrop).MinBy(x => x > 0);
			output.SchemaOutputs = result;
			output.WaterOutletTemperature = result.WaterOutletTemperature;
		}
		catch (Exception ex)
		{
		}


		return output;
	}

	public async Task<WaterDistributionSystemOutputData> GetPerformance(WaterDistributionSystemInputData input, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		WaterDistributionSystemOutputData output = new WaterDistributionSystemOutputData();

		SchemaInputData schemaInputData = new SchemaInputData()
		{
			FluidType = input.FluidType,
			Application = input.Application,
			WaterInletTemperature = input.WaterInletTemperature,
			GlycolPercentage = input.GlycolPercentage,
			Inputs = input.InputCalc
		};

		var result = await Schema.Detect(schemaInputData, cancellationToken);

		output.Capacity = result.Capacity;
		output.DeltaPressureDrop = result.DeltaPressureDrop;
		output.DeltaCapacity = result.DeltaCapacity;
		output.PressureDrop = double.MaxValue;
		foreach (var branch in result.BranchesOutput)
		{
			if (branch.PressureDrop < output.PressureDrop) output.PressureDrop = branch.PressureDrop;
		}

		return output;
	}

	public void Init(InputCalc inputCalc)
	{
		SchemaComponentFromSchemaEntityFactory schemaComponentFromSchemaEntityFactory = new SchemaComponentFromSchemaEntityFactory(inputCalc);

		Schema = schemaComponentFromSchemaEntityFactory.BuildComponent();
		GeneticAlgorithm = new GeneticAlgorithm();

		List<double> flowAve = new List<double>();
		List<double> flowMax = new List<double>();
		List<double> flowMin = new List<double>();
		foreach (var branch in Schema.Branches)
		{
			flowAve.Add(branch.UnitBlock.Unit.PerformancePoints.Select(x => x.Waterflow).Average());
			flowMin.Add(branch.UnitBlock.Unit.PerformancePoints.Select(x => x.Waterflow).Min());
			flowMax.Add(branch.UnitBlock.Unit.PerformancePoints.Select(x => x.Waterflow).Max());
		}
		NominalFlow = Math.Round(flowAve.Average(), 1);
		Maxflow = flowMax.Max();
		Minflow = flowMin.Min();
	}
	public List<CalcData> GetInputCalc()
	{
		return Schema.GetEmptyCalcData();
	}
}

