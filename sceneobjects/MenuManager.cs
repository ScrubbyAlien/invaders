using invaders.interfaces;
using static invaders.Utility;
using static SFML.Window.Keyboard.Key;

namespace invaders.sceneobjects;

public class MenuManager : SceneObject
{
    private List<IClickable> _menuButtons = new();
    private List<IClickable.ClickedEvent> _listeners = new();
    private int _lastIndex = -1;
    private int _pointerIndex;
    private bool _keyPressed;
    private float _holdDown = 0.2f;
    private float _holdDownTimer;
    private bool _firstFrame;

    protected override void Initialize()
    {
        _menuButtons[_pointerIndex].Select();
        _firstFrame = true;
    }

    public override void Destroy()
    {
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].Clicked -= _listeners[i];
        }
    }

    public override void Update(float deltaTime)
    {
        if (_firstFrame && AreAnyKeysPressed([Space, Enter, W, S, Up, Down]))
        {
            return;
        }
        else _firstFrame = false;
        
        if (AreAnyKeysPressed([W, Up]) && !_keyPressed)
        {
            _lastIndex = _pointerIndex;
            _menuButtons[_lastIndex].Unselect();
            _pointerIndex--;
            if (_pointerIndex < 0) _pointerIndex = _menuButtons.Count - 1;
            _menuButtons[_pointerIndex].Select();
            _keyPressed = true;
        }
        else if (AreAnyKeysPressed([S, Down]) && !_keyPressed)
        {
            _lastIndex = _pointerIndex;
            _menuButtons[_lastIndex].Unselect();
            _pointerIndex++;
            if (_pointerIndex >= _menuButtons.Count) _pointerIndex = 0;
            _menuButtons[_pointerIndex].Select();
            _keyPressed = true;
        }
        else if (AreAnyKeysPressed([W, S, Up, Down]))
        {
            _holdDownTimer += deltaTime;
            _keyPressed = true;
        }
        else
        {
            _keyPressed = false;
            _holdDownTimer = 0;
        }

        if (_holdDownTimer >= _holdDown)
        {
            _keyPressed = false;
            _holdDownTimer = 0;
        }

        if (AreAnyKeysPressed([Space, Enter]))
        {
            _menuButtons[_pointerIndex].Click();
        }
    }
    
    public void AddButton(IClickable button, IClickable.ClickedEvent listener)
    {
        _menuButtons.Add(button);
        button.Clicked += listener;
        _listeners.Add(listener); // save listeners on same index for unsubbing in Destroy
    }
    
}