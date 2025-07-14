
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Factories;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.CommonLib;


namespace Daikin.Computation.WaterDistributionSystem.Components.PumpModel;

public class Pump : Component//, IDetectHandler<PumpInputData, PumpOutputData>
{
//    private Link? _motor;
//    private Inverter? _inverter;
//    //private MathTools MathTools = new MathTools();

//    public Pump()
//    {
//    }

//    #region Components

//    public Link? Motor
//    {
//        get
//        {
//            if (_motor is null)
//            {
//                _motor = Components.OfType<Link>().SingleOrDefault();
//            }
//            return _motor;
//        }
//    }
//    public Inverter? Inverter
//    {
//        get
//        {
//            if (_inverter is null)
//            {
//                _inverter = Components.OfType<Inverter>().SingleOrDefault();
//            }
//            return _inverter;
//        }
//    }

//    #endregion

//    protected override void RefreshComponents()
//    {
//        _motor = null;
//        _inverter = null;
//    }

//    public IReadOnlyCollection<Envelope> Envelopes { get; set; } = new List<Envelope>();
//    public IReadOnlyCollection<KFactors> KFactors { get; set; } = new List<KFactors>();

//    public bool HasInverter => Inverter is not null;
//    public bool IsEnvelopeVariableFrequency => Envelopes.Select(e => e.Frequency).Distinct().Count() > 1;

//    public double Poff { get; set; }
//    public double Psb { get; set; }
//    public double Pck { get; set; }
//    public double LoadMin { get; set; }
//#warning "verificare lettura e utilità"
//    public double NominalShaftSpeed { get; set; }
//    public double FrequencyNominal { get; set; }
//    public bool Economizer { get; set; }
//    public double PressureDropHigh { get; set; }
//    public double PressureDropLow { get; set; }
//    public double MinimumPressureRatioECO { get; set; }

//    public IReadOnlyCollection<SoundSpectrumElement> SoundSpectrum { get; set; } = new List<SoundSpectrumElement>();
//    public IReadOnlyCollection<PumpPerformanceCoefficients> CapacityCoefficients { get; set; } = new List<PumpPerformanceCoefficients>();
//    public IReadOnlyCollection<PumpPerformanceCoefficients> PowerInputCoefficients { get; set; } = new List<PumpPerformanceCoefficients>();
//    public IReadOnlyCollection<PumpPerformanceCoefficients> MassFlowCoefficients { get; set; } = new List<PumpPerformanceCoefficients>();
//    public IReadOnlyCollection<PumpPerformanceCoefficients> CurrentCoefficients { get; set; } = new List<PumpPerformanceCoefficients>();


//    #region Public Methods
     

//    public async Task<PumpOutputData> Detect(PumpInputData inputData, CancellationToken cancellationToken)
//    {
//        cancellationToken.ThrowIfCancellationRequested();
//        PumpOutputData outputData = new PumpOutputData();

       

//        return outputData;
//    }

//    #endregion

   
}