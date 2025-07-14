
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Components.UnitModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.LinkModel;
using Daikin.Computation.WaterDistributionSystem.Components.PipeModel;
using Daikin.Computation.WaterDistributionSystem.Components.NodeModel;


namespace Daikin.Computation.WaterDistributionSystem.Components.UnitBlockModel;

public class UnitBlock : Component, IDetectHandler<UnitBlockInputData, UnitBlockOutputData>
{
    #region Properties
    public UnitModel.Unit Unit { get; set; }

    public int Id { get; set; }

    public double Resistance { get; set; }
    public Guid ToId { get; set; }
    //public Guid UniId { get; set; }
    public Guid FromId { get; set; }
    public IReadOnlyCollection<LinkModel.Link> Links { get; set; }
    public IReadOnlyCollection<NodeModel.Node> Nodes { get; set; }

    public UnitBlock()
    {
    }
    protected override void RefreshComponents()
    {
        Links = null;
        Unit = null;
        Nodes = null;
    }
    #endregion



    #region Public Methods
    public async Task<UnitBlockOutputData> Detect(UnitBlockInputData input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        UnitBlockOutputData output = new UnitBlockOutputData();

        UnitInputData unitInputData = new UnitInputData
        {
            WaterFlow = input.WaterFlow,
            WaterInletTemperature = input.WaterInletTemperature,
            FluidType = input.FluidType,
            GlycolPercent = input.GlycolPercentage,
            Application = input.Application
        };
        try
        {
            var result = await Unit.Detect(unitInputData, cancellationToken);
            output.Capacity = result.SensibleCapacity;
            output.WaterOutletTemperature = result.WaterOutletTemperature;
            output.WaterFlow = input.WaterFlow;
            output.PressureDrop = Math.Round(Resistance * Math.Pow(output.WaterFlow, 2), 2);
            output.ThermalCapacity = (output.WaterFlow / 1000) * (output.WaterOutletTemperature + 273.15);
        }catch(Exception ex)
        {

        }
        return output;
    }
    #endregion

    #region Private Methods  

    internal (Guid FromId, Guid ToId) GetFromandToId()
    {
        return (Links.First(l => l.ToId == Unit.Id).FromId, Links.First(l => l.FromId == Unit.Id).ToId);
    }

    internal double GetResistance()
    {
        double output = 0;
        var pipes = Links.Select(x => x.Pipe);
        foreach (var pipe in pipes)
        {
            output += pipe.Resistance;
        }
        output += Unit.Resitance;
        return output;
    }
    #endregion
}
