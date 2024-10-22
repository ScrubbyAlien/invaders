namespace invaders.sceneobjects.renderobjects.gui;

public sealed class TabNavigator(bool continuous = true, bool looping = true, bool horizontal = false) : Navigator(0.2f, continuous, looping, horizontal)
{
    private readonly List<INavigatable> _tabs = new();
    private readonly List<Action> _actions = new();
    
    protected override int Count()
    {
        return _tabs.Count();
    }

    protected override void Initialize()
    {
        if (Count() > 0)
        {
            _tabs[0].Activate();
            _actions[0].Invoke();
        }    
        
    }

    private void ActivateTab(int index)
    {
        _tabs.ForEach(t => t.Deactivate());
        _tabs[index].Activate();
        _actions[index].Invoke();
    }
    
    protected override void SelectNext(int pointer)
    {
        if (Count() > 0)
        {
            ActivateTab(pointer);
        }
    }

    protected override void HandleLastIndex(int last)
    {
        _tabs[last].Deactivate();
    }

    public void AddTab(INavigatable tab, Action tabAction)
    {
        if (!_tabs.Contains(tab))
        {
            _tabs.Add(tab);
            _actions.Add(tabAction);
        }
    }

    public override void SetActiveSelection()
    {
        base.SetActiveSelection();
        _tabs.ForEach(t => t.SetActiveSelection());
        EnableNavigator();
    }

    public override void SetInactiveSelection()
    {
        base.SetInactiveSelection();
        _tabs.ForEach(t => t.SetInactiveSelection());
        DisableNavigator();
    }

    public override void EnableNavigator()
    {
        PointerAction((pointer) =>
        {
            ActivateTab(pointer);
        });
    }

    public override void DisableNavigator()
    {
        _tabs.ForEach(t => t.Deactivate());
    }

    public override void SetIndex(int index)
    {
        base.SetIndex(index);
        ActivateTab(index);
    }
}