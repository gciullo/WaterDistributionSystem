namespace Daikin.Computation.WaterDistributionSystem.DetectData;

public class EnumData
{
    public enum FluidType
    {
        WATER,
        ETHYLENE,
        PROPYLENE
    }
    public enum Control
    {
        ON_OFF,
        VFD,
        BRUSHLESS,
        SPEEDTROLL
    }
    public enum PumpLift { HIGH_LIFT, LOW_LIFT };
    public enum PumpType { GLANDLESS, DRY_MOTOR };
    public enum PipeMaterial
    {
        GalvanizedSteel,
        BlackSteel,
        Copper,
        Iron,
        Pvc,
        Cpvc,
        PolyEthylene,
        PolyPropylene,
    }
    public enum Application
    {
        Cooling,
        Heating,
    }

}

