
using System.Collections.ObjectModel;


namespace Daikin.Computation.WaterDistributionSystem.Components.PumpModel;

public class PumpPerformanceCoefficients : ReadOnlyCollection<double>
{
    public PumpPerformanceCoefficients(IList<double> data, bool isEco, double frequency)
        : base(data)
    {
        IsEco = isEco;
        Frequency = frequency;
    }

    public bool IsEco { get; init; }
    public double Frequency { get; init; }
}
