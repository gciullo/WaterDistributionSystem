using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.LinkModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using System.Drawing;
using System.Linq;
namespace Daikin.Computation.WaterDistributionSystem.Components.GAModel;

internal class GASolution
{
    public List<double> Flows { get; set; } = new();
    public List<int> DiameterIndexes { get; set; } = new();
}

public class GeneticAlgorithm : Component, IDetectHandler<GAInputData, GAOutputData>
{
    public List<CalcData> Solution;
    public List<int> DiameterSolution;
    public WaterDistributionSystem WaterDistributionSystem { get; set; }
    internal Random random = new Random(1);

    private async Task<double> FitnessFunction(GASolution solution, GAInputData inputData, CancellationToken cancellationToken)
    {
        double Fitness = Math.Pow(10, 8);
        cancellationToken.ThrowIfCancellationRequested();
        List<CalcData> input = new List<CalcData>();
        int i = 0;
        foreach (var solutionItem in Solution)
        {
            solutionItem.Id = Solution[i].Id;
            solutionItem.WaterFlow = solution.Flows[i++];
            input.Add(solutionItem);
        };

        int j = 0;
        foreach (var pipe in WaterDistributionSystem.Schema.Pipes)
        {
            var idx = solution.DiameterIndexes[j];
            if (idx < 0 || idx >= pipe.DiametersAdmitted.Count) idx = 0;
            pipe.Diameter = pipe.DiametersAdmitted[idx];
            j++;
        }

        List<CalcData> inputSchema = CreateInputSchema(input);

        inputData.waterDistributionSystemInput.InputCalc = inputSchema;
        try
        {
            var result = await WaterDistributionSystem.GetPerformance(inputData.waterDistributionSystemInput, cancellationToken);

            double maxPressureDrop = 0;
            double maxDeltaCapacity = 0;
            foreach (var deltaPressureDrop in result.DeltaPressureDrop) if (deltaPressureDrop > maxPressureDrop) maxPressureDrop = deltaPressureDrop;
            foreach (var deltaCapacity in result.DeltaCapacity) if (deltaCapacity > maxDeltaCapacity) maxDeltaCapacity = deltaCapacity;

            if (result.Capacity > 0) Fitness = (result.PressureDrop / 100) + maxPressureDrop * 10 + maxDeltaCapacity * 1000;
        }
        catch (Exception ex) { };
        return Fitness;
    }

    internal List<CalcData> CreateInputSchema(List<CalcData> solution)
    {
        List<CalcData> inputSchema = new();

        inputSchema.AddRange(solution);

        List<Guid> linksIn = new();
        foreach (var branch in WaterDistributionSystem.Schema.Branches)
        {
            foreach (var linkInlet in branch.LinksInlet)
            {
                if (linksIn.Contains(linkInlet.FromId)) linksIn.Add(linkInlet.FromId);
            }
        }

        foreach (var linkIn in linksIn)
        {
            List<Guid> unitsInvolved = new();

            foreach (var unitBlock in WaterDistributionSystem.Schema.UnitBlocks)
            {
                if (WaterDistributionSystem.Schema.GetNodeList(unitBlock.Unit.Id, true).Contains(linkIn)) unitsInvolved.Add(unitBlock.Unit.Id);
            }

            List<double> waterFlows = new();

            foreach (var item in solution)
            {
                if (unitsInvolved.Contains(item.Id)) waterFlows.Add(item.WaterFlow);
            }

            inputSchema.Add(new CalcData
            {
                Id = linkIn,
                WaterFlow = Math.Round(waterFlows.Sum(), 3)
            });
        }

        List<Guid> linksOut = new();
        foreach (var branch in WaterDistributionSystem.Schema.Branches)
        {
            foreach (var linkOutet in branch.LinksOutlet)
            {
                if (linksOut.Contains(linkOutet.FromId)) linksIn.Add(linkOutet.FromId);
            }
        }

        foreach (var linkOut in linksOut)
        {
            List<Guid> unitsInvolved = new();

            foreach (var unitBlock in WaterDistributionSystem.Schema.UnitBlocks)
            {
                if (WaterDistributionSystem.Schema.GetNodeList(unitBlock.Unit.Id, false).Contains(linkOut)) unitsInvolved.Add(unitBlock.Unit.Id);
            }

            List<double> waterFlows = new();

            foreach (var item in solution)
            {
                if (unitsInvolved.Contains(item.Id)) waterFlows.Add(item.WaterFlow);
            }

            inputSchema.Add(new CalcData
            {
                Id = linkOut,
                WaterFlow = Math.Round(waterFlows.Sum(), 3)
            });
        }

        return inputSchema;
    }

    // Generazione di una soluzione casuale
    private List<double> GenerateRandomSolution(List<CalcData> Solution, IEnumerable<Guid> idUnits, List<double> minValue, List<double> maxValue, bool allowSwitchOffUnit)
    {
        List<double> solution = new List<double>();

        foreach (var solutionItem in Solution)
        {
            var solutionIndex = Solution.IndexOf(solutionItem);
            var x = Math.Max(Math.Round(random.NextDouble(), 3), 0.1);
            solutionItem.WaterFlow = Math.Round((x * (maxValue.ElementAt(solutionIndex) - minValue.ElementAt(solutionIndex)) + minValue.ElementAt(solutionIndex)), 3);

            if (allowSwitchOffUnit && solutionItem.WaterFlow < minValue.ElementAt(solutionIndex))
                solutionItem.WaterFlow = 0;

            solution.Add(solutionItem.WaterFlow);
        }
        solution = CheckFlow(Solution, idUnits, minValue);
        return solution;
    }

    private List<int> GenerateRandomDiameterSolution()
    {
        List<int> diameters = new();
        foreach (var pipe in WaterDistributionSystem.Schema.Pipes)
        {
            diameters.Add(random.Next(pipe.DiametersAdmitted.Count));
        }
        return diameters;
    }

    private List<int> GenerateFirstDiameterSolution()
    {
        List<int> diameters = new();
        foreach (var pipe in WaterDistributionSystem.Schema.Pipes)
        {
            if (pipe.Diameter.HasValue)
                diameters.Add(Math.Max(0, pipe.DiametersAdmitted.IndexOf(pipe.Diameter.Value)));
            else
                diameters.Add(0);
        }
        return diameters;
    }
    private List<double> GenerateFirstSolution(List<CalcData> Solution, IEnumerable<Guid> idUnits, double nominalValue, List<double> minValue, List<double> maxValue)
    {
        List<double> solution = new List<double>();
        foreach (var solutionItem in Solution)
        {
            var solutionIndex = Solution.IndexOf(solutionItem);
            var x = Math.Max(Math.Round(random.NextDouble(), 2), 0.1);
            var units = WaterDistributionSystem.Schema.UnitBlocks.Where(x => x.Unit.Id == solutionItem.Id);
            if (units.Count() > 0)
            {
                solutionItem.WaterFlow = Math.Round((minValue.ElementAt(solutionIndex) + maxValue.ElementAt(solutionIndex)) / 2, 2);
            }
            else
            {
                solutionItem.WaterFlow = Math.Round((x * (maxValue.ElementAt(solutionIndex) - minValue.ElementAt(solutionIndex)) + minValue.ElementAt(solutionIndex)), 2);
            }
            solution.Add(solutionItem.WaterFlow);
        }
        solution = CheckFlow(Solution, idUnits, minValue);
        return solution;
    }
    private (List<double> MinFlowArray, List<double> MaxFlowArray) GenerateMinAndMaxFlowArray(List<CalcData> Solution, IEnumerable<Guid> idUnits, double minValue, double maxValue)
    {
        List<double> minFlowArray = new List<double>();
        List<double> maxFlowArray = new List<double>();
        foreach (var solutionItem in Solution)
        {
            var solutionMinFlow = minValue;
            var solutionMaxFlow = maxValue;

            if (idUnits.Contains(solutionItem.Id))
            {
                foreach (var unit in WaterDistributionSystem.Schema.UnitBlocks)
                {
                    if (unit.Unit.Id == solutionItem.Id)
                    {
                        double maxPerfPointFlow = 0;
                        double minPerfPointFlow = Double.MaxValue;
                        foreach (var point in unit.Unit.PerformancePoints)
                        {
                            if (point.Waterflow < minPerfPointFlow) minPerfPointFlow = point.Waterflow;
                            if (point.Waterflow > maxPerfPointFlow) maxPerfPointFlow = point.Waterflow;
                        }

                        var maxFlowConsideringDeltaT = unit.Unit.GetFlowFromDeltaTemperature(3);    //Delta T minimo = 3°C
                        var minFlowConsideringDeltaT = unit.Unit.GetFlowFromDeltaTemperature(8);    //Delta T minimo = 8°C

                        solutionMaxFlow = maxFlowConsideringDeltaT <= minPerfPointFlow ? maxPerfPointFlow : Double.Min(maxPerfPointFlow, maxFlowConsideringDeltaT);
                        solutionMinFlow = minFlowConsideringDeltaT >= maxPerfPointFlow ? minPerfPointFlow : Double.Max(minPerfPointFlow, minFlowConsideringDeltaT);

                        break;
                    }
                }
            }

            minFlowArray.Add(solutionMinFlow);
            maxFlowArray.Add(solutionMaxFlow);
        }

        return (minFlowArray, maxFlowArray);
    }
    private List<double> CheckFlow(List<CalcData> Solution, IEnumerable<Guid> idUnits, List<double> minValue, bool allowSwitchOffUnit = false)
    {
        List<double> solution = new();
        foreach (var solutionItem in Solution)
        {
            var solutionIndex = Solution.IndexOf(solutionItem);
            if (idUnits.Contains(solutionItem.Id) && allowSwitchOffUnit && solutionItem.WaterFlow < minValue.ElementAt(solutionIndex))
                solutionItem.WaterFlow = 0;
        }

        solution = Solution.Select(x => x.WaterFlow).ToList();
        foreach (var point in Solution.Where(s => !idUnits.Contains(s.Id)))
        {
            var unitsInvolved = WaterDistributionSystem.Schema.UnitBlocks.Where(u =>
                                    WaterDistributionSystem.Schema.GetNodeList(u.Unit.Id, true)
                                    .Union(WaterDistributionSystem.Schema.GetNodeList(u.Unit.Id, false))
                                    .Contains(point.Id)).Select(x => x.Unit.Id);

            var waterFlows = Solution.Where(s => unitsInvolved.Contains(s.Id))
                          .Select(s => solution.ElementAt(Solution.IndexOf(s)))
                          .ToList();

            solution[Solution.IndexOf(point)] = Math.Round(waterFlows.Sum(), 3);
        }
        foreach (var solutionItem in Solution)
        {
            solutionItem.WaterFlow = solution.IndexOf(solutionItem.WaterFlow);
        }
        return solution;
    }

    private List<double> CheckFlow(List<double> flow, IEnumerable<Guid> idUnits, List<double> minValue, bool allowSwitchOffUnit = false)
    {
        List<double> solution = flow.ToList();
        List<int> indices = new();
        List<int> complementaryIndices = new();
        foreach (var sol in Solution)
        {
            if (idUnits.Contains(sol.Id)) indices.Add(Solution.IndexOf(sol));
            else complementaryIndices.Add(Solution.IndexOf(sol));
        }

        foreach (var index in indices)
        {
            if (index >= 0 && index < solution.Count)
            {
                if (solution[index] < minValue[index] && allowSwitchOffUnit)
                    solution[index] = 0;
            }
        }
        foreach (var index in complementaryIndices)
        {
            var point = Solution[index];

            List<Guid> unitsInvolved = new();
            foreach (var unit in WaterDistributionSystem.Schema.UnitBlocks)
            {
                List<Guid> nodeList = new();
                nodeList.AddRange(WaterDistributionSystem.Schema.GetNodeList(unit.Unit.Id, true));
                nodeList.AddRange(WaterDistributionSystem.Schema.GetNodeList(unit.Unit.Id, false));

                if (nodeList.Contains(point.Id)) unitsInvolved.Add(unit.Unit.Id);
            }

            double waterFlows = 0;
            foreach (var sol in Solution)
            {
                if (unitsInvolved.Contains(sol.Id)) waterFlows += solution.ElementAt(Solution.IndexOf(sol));
            }

            solution[index] = Math.Round(waterFlows, 3);
        }
        return solution;
    }
    // Algoritmo Genetico
    public async Task<GAOutputData> Detect(GAInputData inputData, CancellationToken cancellationToken)
    {
        List<GASolution> population = new List<GASolution>();
        List<double> BestFitness = new List<double>();
        Solution = WaterDistributionSystem.GetInputCalc();
        DiameterSolution = new List<int>();
        GAOutputData output = new GAOutputData();
        var idUnits = WaterDistributionSystem.Schema.UnitBlocks.Select(x => x.Unit.Id);

        (var minFlowArray, var maxFlowArray) = GenerateMinAndMaxFlowArray(Solution, idUnits, inputData.minValue, inputData.maxValue);

        var first = new GASolution
        {
            Flows = GenerateFirstSolution(Solution, idUnits, inputData.nominalValue, minFlowArray, maxFlowArray),
            DiameterIndexes = GenerateFirstDiameterSolution()
        };
        population.Add(first);

        for (int i = 1; i < inputData.populationSize; i++)
        {
            population.Add(new GASolution
            {
                Flows = GenerateRandomSolution(Solution, idUnits, minFlowArray, maxFlowArray, inputData.waterDistributionSystemInput.AllowSwitchOffUnit),
                DiameterIndexes = GenerateRandomDiameterSolution()
            });
        }

        double alpha = 0.1;
        for (int gen = 0; gen < inputData.generations; gen++)
        {
            // Valutazione
            var fitnessScores = new List<double>();
            foreach (var solution in population)
            {
                fitnessScores.Add(await FitnessFunction(solution, inputData, cancellationToken));
            }
            // Selection
            population = population.OrderBy(solution => fitnessScores[population.IndexOf(solution)]).ToList();
            output.Solution = population.First().Flows;
            output.Fitness = await FitnessFunction(population.First(), inputData, cancellationToken);
            BestFitness.Add(output.Fitness);
            if (StopRun(BestFitness, inputData.generations)) break;

            var selected = population.Take((int)(inputData.populationSize * 0.2)).ToList();

            if (gen > inputData.generations / 2.0) alpha = 0.5;
            // Crossover e Mutazione
            var children = new List<GASolution>();
            while (children.Count < inputData.populationSize - selected.Count)
            {
                var parent1 = selected[random.Next(selected.Count)];
                var parent2 = selected[random.Next(selected.Count)];

                int count = 0;
                // Ensure parent2 is different from parent1
                while (parent1 == parent2 && count < selected.Count)
                {
                    parent2 = selected[random.Next(selected.Count)];
                    count++;
                }
                var flowsChild = BLXAlphaCrossover(parent1.Flows, parent2.Flows, alpha, minFlowArray, maxFlowArray);
                flowsChild = CheckFlow(flowsChild, idUnits, minFlowArray);
                var mutationFlows = GaussianMutation(flowsChild, inputData.mutationRate, minFlowArray, maxFlowArray);
                mutationFlows = CheckFlow(mutationFlows, idUnits, minFlowArray);
                var diamChild = CrossoverDiameters(parent1.DiameterIndexes, parent2.DiameterIndexes);
                var diamMutation = MutateDiameters(diamChild, inputData.mutationRate);

                // Check if the mutated child is a twin of any existing solution
                bool isTwin = selected.Any(existing => AreTwins(mutationFlows, existing.Flows)) ||
                              children.Any(existing => AreTwins(mutationFlows, existing.Flows));

                if (!isTwin)
                {
                    children.Add(new GASolution { Flows = mutationFlows, DiameterIndexes = diamMutation });
                }
            }
            // Sostituzione
            population = selected.Concat(children).ToList();


        }
        output.Solution = population.First().Flows;
        CheckFlow(output.Solution, idUnits, minFlowArray);
        output.Fitness = await FitnessFunction(population.First(), inputData, cancellationToken);

        return output;
    }
    private bool StopRun(List<double> BestFitness, int MaxGeneration)
    {
        bool stop = false;
        int windowSize = (int)(MaxGeneration / 10.0);
        double slopeThreshold = 0.001;
        if (BestFitness.Count >= windowSize)
        {
            double recentAverageSlope = (BestFitness.Last() - BestFitness[BestFitness.Count - windowSize]) / windowSize;
            if (Math.Abs(recentAverageSlope) <= slopeThreshold) stop = true;
        }
        return stop;
    }
    bool AreTwins(List<double> solution1, List<double> solution2)
    {
        // Implement your logic to determine if two solutions are "twins"
        // For example, you could check if every element in the lists is equal
        return solution1.SequenceEqual(solution2);
    }
    List<double> GaussianMutation(List<double> solution, double mutationRate, List<double> minValues, List<double> maxValues)
    {
        double stdDev = 0;
        foreach (var maxValue in maxValues)
        {
            foreach (var minValue in minValues)
            {
                var value = (maxValue - minValue) * 0.1;

                if (value > stdDev) stdDev = value;
            }
        }


        List<double> result = new();
        foreach (var gene in solution)
        {
            if (random.NextDouble() < mutationRate / 100)
            {
                var geneIndex = solution.IndexOf(gene);
                double change = random.NextGaussian() * stdDev; // NextGaussian() should generate a Gaussian random number
                result.Add(Math.Clamp(gene + change, minValues.ElementAt(geneIndex), maxValues.ElementAt(geneIndex)));
                continue;
            }
            result.Add(gene);
        }

        return result;
    }
    List<double> BLXAlphaCrossover(List<double> parent1, List<double> parent2, double alpha, List<double> minValue, List<double> maxValue)
    {
        if (parent1.Count != parent2.Count) return new();

        List<double> result = new();
        foreach (var p1 in parent1)
        {
            var p2 = parent2.IndexOf(p1);

            var p1Index = parent1.IndexOf(p1);

            double range = Math.Abs(p1 - p2);
            double min = Math.Min(p1, p2) - alpha * range;
            double max = Math.Max(p1, p2) + alpha * range;
            double flow = Math.Round(Math.Clamp(min + random.NextDouble() * (max - min), minValue.ElementAt(p1Index), maxValue.ElementAt(p1Index)), 3);

            result.Add(flow);
        }

        return result;
    }

    List<int> CrossoverDiameters(List<int> parent1, List<int> parent2)
    {
        List<int> result = new();
        for (int i = 0; i < parent1.Count; i++)
        {
            result.Add(random.NextDouble() < 0.5 ? parent1[i] : parent2[i]);
        }
        return result;
    }

    List<int> MutateDiameters(List<int> diameters, double mutationRate)
    {
        List<int> result = new();
        int index = 0;
        foreach (var d in diameters)
        {
            if (random.NextDouble() < mutationRate / 100)
            {
                var pipe = WaterDistributionSystem.Schema.Pipes.ElementAt(index);
                result.Add(random.Next(pipe.DiametersAdmitted.Count));
            }
            else result.Add(d);
            index++;
        }
        return result;
    }

}
public static class RandomExtensions
{
    public static double NextGaussian(this Random random, double mean = 0, double standardDeviation = 1)
    {
        double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        return mean + standardDeviation * randStdNormal; //random normal(mean,stdDev^2)
    }
}
