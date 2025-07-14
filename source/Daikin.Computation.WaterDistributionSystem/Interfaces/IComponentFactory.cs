
using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

namespace Daikin.Computation.WaterDistributionSystem.Interfaces;

public interface IComponentFactory<out T> where T : Component
{
    T BuildComponent();
}
