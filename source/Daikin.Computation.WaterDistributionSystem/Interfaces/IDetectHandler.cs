namespace Daikin.Computation.WaterDistributionSystem.Interfaces;

public interface IDetectHandler<in TIn, TOut>
    where TIn : IDetectInput<TOut>
{
    Task<TOut> Detect(TIn input, CancellationToken cancellationToken);
}
