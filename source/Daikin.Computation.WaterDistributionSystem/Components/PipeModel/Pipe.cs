

using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;


namespace Daikin.Computation.WaterDistributionSystem.Components.PipeModel;


public class Pipe : Component, IDetectHandler<PipeInputData, PipeOutputData>
{
    public Guid Id { get; set; }
    public double Length { get; set; }
    public double DeltaHeight { get; set; }
    public double Resistance { get; set; }
    public int NumberOfBends { get; set; }
    public double? Diameter { get; set; }
    public EnumData.PipeMaterial Material { get; set; }
    double Factor = 1.2;

    public Pipe()
    {
    }
    public async Task<PipeOutputData> Detect(PipeInputData input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PipeOutputData output = new PipeOutputData();
        output.PressureDrop = Math.Round(Resistance * Math.Pow(input.WaterFlow, 2), 2);

        return output;
    }

    #region Private Method 
    internal double GetResistance()
    {
        List<double> K = new List<double>();
        var output = GetPressureDrop();

        foreach (var point in output)
        {
            K.Add(point.PressureDrop / Math.Pow(point.Waterflow, 2));
        }
        Resistance = K.Average();
        return Resistance;
    }

    private List<PipePerfomance> GetPressureDrop()
    {
        List<PipePerfomance> output = new List<PipePerfomance>();
        List<double> Flow = new List<double> { 0.01, 0.03, 0.05, 0.07, 0.1, 0.3, 0.5, 0.6, 0.7, 1.2, 2 };
        foreach (var flow in Flow)
        {
            PipePerfomance perfomance = new PipePerfomance();
            perfomance.Waterflow = flow;
            ////cadente [m/km]
            //double J = 6.81 * Math.Pow(10, 8) * (Math.Pow(flow, 1.82) / Math.Pow(Diameter!.Value, 4.71));

            //perfomance.PressureDrop = J * Factor * (Length / 1000) * 9.81;

            var c = Material switch
            {
                EnumData.PipeMaterial.GalvanizedSteel => 120,
                EnumData.PipeMaterial.BlackSteel => 120,
                EnumData.PipeMaterial.Copper => 140,
                EnumData.PipeMaterial.Iron => 100,
                EnumData.PipeMaterial.Pvc => 150,
                EnumData.PipeMaterial.Cpvc => 150,
                EnumData.PipeMaterial.PolyEthylene => 140,
                EnumData.PipeMaterial.PolyPropylene => 150,
                _ => 0,
            };

            //Calcolo perdite distribuite
            var J = 10.675 * Math.Pow(flow / 1000, 1.852) / (Math.Pow(c, 1.852) * Math.Pow(Diameter!.Value / 1000, 4.8704)); //espresso in m/m
            var pressureDrop = J * Length * 9.81; //in kPa

            //Calcolo perdite concentrate
            var alpha = -0.078 * Math.Log(Diameter!.Value) + 0.6815;
            var bendsPressureDrop = NumberOfBends * (alpha * Math.Pow(flow / 1000 / (Math.PI * Math.Pow(Diameter!.Value / 1000, 2) / 4), 2) / 2);
            pressureDrop += bendsPressureDrop;

            perfomance.PressureDrop = pressureDrop;

            output.Add(perfomance);
        }
        return output;
    }
    #endregion


}