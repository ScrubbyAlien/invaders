namespace invaders.sceneobjects.gui;

public sealed class TabNavigator(bool looping = true, bool horizontal = false) : Navigator(0.2f, looping, horizontal)
{
    private List<INavigatable> _tabs = new();
    private List<Action> _actions = new();
    
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

    protected override void SelectNext(int pointer)
    {
        if (Count() > 0)
        {
            _tabs[pointer].Activate();
            _actions[pointer].Invoke();
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
        _tabs[0].Activate();
        _actions[0].Invoke();
    }

    public override void DisableNavigator()
    {
        _tabs.ForEach(t => t.Deactivate());
    }

}