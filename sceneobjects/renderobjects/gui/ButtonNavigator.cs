using SFML.Audio;
using SFML.Window;
using static invaders.Utility;
using static SFML.Window.Keyboard.Key;

namespace invaders.sceneobjects.renderobjects.gui;

public sealed class ButtonNavigator(bool looping = true, bool horizontal = false) : Navigator(0.2f, looping, horizontal)
{
    private readonly List<IClickable> _buttons = new();
    private readonly List<Action> _listeners = new();
    
    private static readonly Keyboard.Key[] _selectKeys = [Enter, Space];

    private Sound _clickSound = AssetManager.LoadSound("click1");

    
    protected override List<Keyboard.Key> _allKeys
    {
        get
        {
            List<Keyboard.Key> r = base._allKeys;
            r.AddRange(_selectKeys);
            return r;
        }
    }

    protected override int Count()
    {
        return _buttons.Count();
    }

    protected override void Initialize()
    {
        if (Active) Scene.DeferredCall(_buttons[0], "Select", []);
    }

    public override void Destroy()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].Clicked -= _listeners[i];
        }
    }

    protected override void NavigatorUpdate(float deltaTime)
    {
        if (AreAnyKeysPressed([Space, Enter]))
        {
            PointerAction(pointer =>
            {
                _clickSound.Play();
                _buttons[pointer].Click();
            });
        }
    }

    protected override void SelectNext(int pointer)
    {
        _buttons[pointer].Select();
    }

    protected override void HandleLastIndex(int last)
    {
        _buttons[last].Deselect();
        
    }

    public override void SetActiveSelection()
    {
        base.SetActiveSelection();
        EnableNavigator();
    }

    public override void SetInactiveSelection()
    {
        base.SetInactiveSelection();
        DisableNavigator();
    }
    
    public override void EnableNavigator()
    {
        _buttons.ForEach(b => b.SetActiveSelection());
        _buttons[0].Select();
    }

    public override void DisableNavigator()
    {
        _buttons.ForEach(b => b.SetInactiveSelection());
    }


    public void AddButton(IClickable button, Action listener)
    {
        if (!_buttons.Contains(button))
        {
            button.Activate();
            _buttons.Add(button);
            button.Clicked += listener;
            _listeners.Add(listener); // save listeners on same index for unsubbing in Destroy
        }
    }
}