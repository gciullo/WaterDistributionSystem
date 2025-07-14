using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;


namespace Daikin.Computation.WaterDistributionSystem.Builders;

public abstract class ComponentBuilder
{
    protected readonly ComponentBuilder? ParentComponentBuilder;
    protected readonly Component? ParentComponent;

    protected Component Component = null!;

    public ComponentBuilder()
        : this(null, null)
    {
    }

    public ComponentBuilder(ComponentBuilder? parentComponentBuilder, Component? parentComponent)
    {
        ParentComponentBuilder = parentComponentBuilder;
        ParentComponent = parentComponent;
    }  
    public PipeBuilder AddPipe()
    {
        return new PipeBuilder(this, Component);
    }
    public UnitBuilder AddUnit()
    {
        return new UnitBuilder(this, Component);
    }
    public NodeBuilder AddNode()
    {
        return new NodeBuilder(this, Component);
    }
    public PumpBuilder AddPump()
    {
        return new PumpBuilder(this, Component);
    }
    public UnitBlockBuilder AddUnitBlock()
    {
        return new UnitBlockBuilder(this, Component);
    }
    public LinkBuilder AddLink()
    {
        return new LinkBuilder(this, Component);
    }




}

public abstract class ComponentBuilder<T> : ComponentBuilder
    where T : Component, new()
{
    protected new T Component => (T)base.Component;

    public ComponentBuilder(ComponentBuilder? parentComponentBuilder, Component? parentComponent)
        : base(parentComponentBuilder, parentComponent)
    {
        base.Component = new T();
    }

    public virtual BT FinishComponent<BT>() where BT : ComponentBuilder
    {
        ParentComponent!.AddComponent(Component);
        return (BT)ParentComponentBuilder!;
    }

    public T Build()
    {
        return Component;
    }
}
