namespace Daikin.Computation.WaterDistributionSystem.DetectData;

public abstract record BaseOutputData
{
    protected BaseOutputData() { }

    public Enumeration ErrorCode { get; set; } = BaseErrorCode.NoError;
}

public class BaseErrorCode : Enumeration
{
    private BaseErrorCode(int id, string name)
       : base(id, name)
    { }

    public static BaseErrorCode NoError => new BaseErrorCode(0, nameof(NoError));
}
