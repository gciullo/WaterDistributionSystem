using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Interfaces;


namespace Daikin.Computation.WaterDistributionSystem.Components.PumpModel;

public record PumpInputData : IDetectInput<PumpOutputData>
{
    public double EvaporatingTemperature { get; set; }
    public double CondensingTemperature { get; set; }
    public double Load { get; set; }
}

public record PumpOutputData : BaseOutputData
{
    public double Capacity { get; set; }
    public double PowerInput { get; set; }
    public double PowerInputTherm { get; set; }
    public double SST { get; set; }
    public double SDT { get; set; }
    public double PressureDropDsch { get; set; }
    public double PressureDropSuction { get; set; }
    public double PressureRatio { get; set; }
    public double InverterEfficency { get; set; }
    public double SoundPower { get; set; }
    public bool ECO { get; set; }
    public double Kcc { get; set; }
    public double Kpi { get; set; }
    public double MaxMotorCurrent { get; set; }
    public double MaxCurrent { get; set; }
    public double Current { get; set; }
    public double MotorCurrent { get; set; }
    public bool UnloadMaxCurrent { get; set; }
    public int UnloadCircuit { get; set; }
}

public class PumpErrorCode : Enumeration
{
    private static int BaseId = 1000;

    public static PumpErrorCode LoadTooLow = new PumpErrorCode(BaseId + 0, nameof(LoadTooLow));
    public static PumpErrorCode MaxCurrentTooHigh = new PumpErrorCode(BaseId + 6, nameof(MaxCurrentTooHigh));

    private PumpErrorCode(int id, string name)
        : base(id, name)
    { }
}
