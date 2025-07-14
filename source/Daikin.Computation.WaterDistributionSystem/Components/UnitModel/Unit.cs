using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra;
using System.Xml.Linq;

namespace Daikin.Computation.WaterDistributionSystem.Components.UnitModel;

public class Unit : Component, IDetectHandler<UnitInputData, UnitOutputData>
{
    public Unit()
    {
    }
    #region Components
    public double Resitance { get; set; }
    public Guid RoomId { get; set; }
    public Guid Id { get; init; }
    public IReadOnlyCollection<PerformancePoint> PerformancePoints { get; set; }
    public IReadOnlyCollection<double> SensCapacityCoefficients { get; set; }
    public IReadOnlyCollection<double> TotalCapacityCoefficients { get; set; }
    public (double c, double n) FlowExpCoefficientsFromDeltaTemperature { get; set; }
    #endregion

    #region Private Methods
    private double GetFlowFromPressureDrop(double pressureDrop)
    {
        return Math.Round(Math.Sqrt(pressureDrop / Resitance), 3);
    }
    private double GetSensibleCapacity(double waterFlow)
    {
        double Cap = 0;
        //if (waterFlow < PerformancePoints.Select(x=>x.Waterflow).Min()) return Cap;
        int i = 0;
        foreach (var coeff in SensCapacityCoefficients)
        {
            Cap += coeff * Math.Pow(waterFlow, i);
            i++;
        }
        return Math.Round(Cap, 3);
    }
    private double GetTotalCapacity(double waterFlow)
    {
        double Cap = 0;
        if (waterFlow < PerformancePoints.Select(x => x.Waterflow).Min()) return Cap;
        int i = 0;
        foreach (var coeff in TotalCapacityCoefficients)
        {
            Cap += coeff * Math.Pow(waterFlow, i);
            i++;
        }
        return Math.Round(Cap, 3);
    }
    private double GetWaterOutletTemperature(double WaterInletTemperature, DetectData.EnumData.FluidType FluidType, double GlycolPercent, double Capacity, double WaterFlow, DetectData.EnumData.Application application)
    {
        double Tw = 0;
        var fluid = GetWater();
        var cp = (fluid.Cpbulk(WaterInletTemperature, FluidType.ToString(), GlycolPercent)) / 1000;
        var rho = fluid.Rhobulk(WaterInletTemperature, FluidType.ToString(), GlycolPercent);

        if (WaterFlow > 0)
        {
            return application switch
            {
                DetectData.EnumData.Application.Cooling => Math.Round(WaterInletTemperature + (Capacity / (cp * rho * (WaterFlow / 1000))), 2),
                DetectData.EnumData.Application.Heating => Math.Round(WaterInletTemperature - (Capacity / (cp * rho * (WaterFlow / 1000))), 2),
                _ => throw new InvalidCastException()
            };
        }
        else return WaterInletTemperature;
    }
    #endregion

    #region Public Methods
    public async Task<UnitOutputData> Detect(UnitInputData input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        UnitOutputData output = new UnitOutputData();

        output.WaterFlow = input.WaterFlow.Value;
        if (input.PressureDrop != null) GetFlowFromPressureDrop(input.PressureDrop!.Value);
        output.PressureDrop = Math.Round(Resitance * Math.Pow(output.WaterFlow, 2), 2);
        output.SensibleCapacity = Math.Max(GetSensibleCapacity(output.WaterFlow), 0);
        output.TotalCapacity = Math.Max(GetTotalCapacity(output.WaterFlow), 0);

        output.WaterOutletTemperature = GetWaterOutletTemperature(input.WaterInletTemperature, input.FluidType, input.GlycolPercent, output.TotalCapacity, output.WaterFlow, input.Application);
        return output;
    }
    internal double[] GetSensibleCapacitycoeff(int polyOrder)
    {
        double R = 0;
        var x = PerformancePoints.Select(x => x.Waterflow).ToArray();
        var y = PerformancePoints.Select(x => x.SensibleCapacity).ToArray();
        return MathTools.PolynomialRegTools.PolynomialFitDynamic(x, y, polyOrder, out R);
    }
    internal double[] GetTotalCapacitycoeff(int polyOrder)
    {
        double R = 0;
        var x = PerformancePoints.Select(x => x.Waterflow).ToArray();
        var y = PerformancePoints.Select(x => x.TotalCapacity).ToArray();
        return MathTools.PolynomialRegTools.PolynomialFitDynamic(x, y, polyOrder, out R);
    }
    internal (double c, double n) GetFlowExpCoeffFromDeltaTemperature()
    {
        var x = PerformancePoints.Select(x => x.DeltaTemperature).ToArray();
        var y = PerformancePoints.Select(x => x.Waterflow).ToArray();
        return MathTools.ExponentialRegTools.FitExponential(x, y);
    }
    internal double GetResistance()
    {
        List<double> K = new List<double>();

        foreach (var point in PerformancePoints)
        {
            K.Add(point.PressureDrop / Math.Pow(point.Waterflow, 2));
        }
        return K.Average();
    }
    internal double GetFlowFromDeltaTemperature(double deltaTemperature)
    {
        return FlowExpCoefficientsFromDeltaTemperature.c * Math.Pow(Math.E, FlowExpCoefficientsFromDeltaTemperature.n * deltaTemperature);
    }
    #endregion
}