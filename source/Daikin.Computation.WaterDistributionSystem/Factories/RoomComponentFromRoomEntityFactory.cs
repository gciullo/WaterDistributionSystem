using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
namespace Daikin.Computation.WaterDistributionSystem.Factories;

public class RoomComponentFromRoomEntityFactory : Component, IComponentFactory<Components.RoomModel.Room>
{
    private readonly Daikin.Computation.WaterDistributionSystem.Room _entity;
    public RoomComponentFromRoomEntityFactory(Daikin.Computation.WaterDistributionSystem.Room entity)
    {
        _entity = entity;
    }
    public Components.RoomModel.Room BuildComponent()
    {
        Components.RoomModel.Room component = new Components.RoomModel.Room()
        {
            Id = _entity.Id,
            Capacity = _entity.Capacity,
            CapacityTolerance = _entity.CapacityTolerance
        };       
        return component;
    }
}


