using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class UnitComponentFromUnitEntityFactory : Component, IComponentFactory<Components.UnitModel.Unit>
{
    private readonly Unit _entity;
    public UnitComponentFromUnitEntityFactory(Unit entity)
    {
        _entity = entity;
    }
    public Components.UnitModel.Unit BuildComponent()
    {
        Components.UnitModel.Unit component = new Components.UnitModel.Unit()
        {
            Id = _entity.Id
        };
        IEnumerable<Components.UnitModel.PerformancePoint> Performances = _entity.PerformancePoints.Select(e =>
        {
            return new Components.UnitModel.PerformancePoint(e.Waterflow/3600, e.PressureDrop, e.Capacity, e.CapacityTotal, e.DeltaTemperature);
        });
        component.PerformancePoints = Performances.ToList();
        component.SensCapacityCoefficients = component.GetSensibleCapacitycoeff(2);
        component.TotalCapacityCoefficients = component.GetTotalCapacitycoeff(2);
        component.FlowExpCoefficientsFromDeltaTemperature = component.GetFlowExpCoeffFromDeltaTemperature();
        component.Resitance = component.GetResistance();
        component.RoomId = _entity.RoomId;
        return component;
    }
}


