using Daikin.Computation.CommonLib;
using Daikin.Computation.CommonLib.MathToolsModel;

namespace Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;

public abstract class Component
{
    public MathTools MathTools = new MathTools();

    public string? Name { get; set; }
    
    public WaterProp? Fluid { get; set; }

    public WaterProp? GetWater()
    {
        return new WaterProp();
    }  

    protected Component()
    {
        _components = new List<Component>();
    }

    private List<Component> _components;
    public IReadOnlyCollection<Component> Components => _components.AsReadOnly();

    public void AddComponent(Component component)
    {
        _components.Add(component);
        OnComponentsListChanged();
    }
    public void RemoveComponent(Component component)
    {
        _components.Remove(component);
        OnComponentsListChanged();
    }

    protected virtual void OnComponentsListChanged()
    {
        RefreshComponents();
    }
    protected virtual void RefreshComponents() { }

    
}
