using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.NodeModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;

namespace Daikin.Computation.WaterDistributionSystem.Components.RoomModel;

public class Room : Component, IDetectHandler<RoomInputData, RoomOutputData>
{
    public Guid Id { get; set; }
    public double  Capacity { get; set;}
    public double CapacityTolerance { get; set; }

    private IReadOnlyCollection<UnitModel.Unit>? _units;
    public IReadOnlyCollection<UnitModel.Unit> Units
    {
        get
        {
            if (_units is null)
            {
                _units = Components.OfType<UnitModel.Unit>().ToList().AsReadOnly();
            }
            return _units;
        }
    }

    private IReadOnlyCollection<LinkModel.Link>? _links;
    public IEnumerable<LinkModel.Link> Links
    {
        get
        {
            if (_links is null)
            {
                _links = Components.OfType<LinkModel.Link>().ToList().AsReadOnly();
            }
            return _links;
        }
    }

    private IReadOnlyCollection<PipeModel.Pipe>? _pipes;
    public IReadOnlyCollection<PipeModel.Pipe> Pipes
    {
        get
        {
            if (_pipes is null)
            {
                _pipes = Components.OfType<PipeModel.Pipe>().ToList().AsReadOnly();
            }
            return _pipes;
        }
    }

    public async Task<RoomOutputData> Detect(RoomInputData inputData, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new RoomOutputData();
    }
    
}
