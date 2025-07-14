using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.SchemaModel;

namespace Daikin.Computation.WaterDistributionSystem.Components.GAModel;

public record GAInputData : BaseInputData, IDetectInput<GAOutputData>
{
    public int populationSize { get; set; } = 50;
    //public int numVariables { get; set; } = 6; // Numero di variabili   
    public int generations { get; set; } = 100;
    public double mutationRate { get; set; } = 5; // Valore massimo per le variabili
    public WaterDistributionSystemInputData waterDistributionSystemInput { get; set; }    
    public double minValue { get; set; } = 0.01; // Valore minimo per le variabili
    public double maxValue { get; set; } = 500; // Valore massimo per le variabili
    public double nominalValue { get; set; }
    public List<Guid> IdUnit { get; set; }
}

public record GAOutputData : BaseOutputData
{
    public List<double> Solution { get; set; }  
    public double Fitness { get; set; }   
}

public class GAErrorCode : Enumeration
{
    private static int BaseId = 1000;

    public static GAErrorCode LoadTooLow = new GAErrorCode(BaseId + 0, nameof(LoadTooLow));   

    private GAErrorCode(int id, string name)
        : base(id, name)
    { }
}
