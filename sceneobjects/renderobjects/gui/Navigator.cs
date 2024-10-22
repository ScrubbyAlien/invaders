using SFML.Audio;
using SFML.Window;
using static SFML.Window.Keyboard.Key;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects.gui;

/// <summary>
/// Navigator represents classes that need to navigate between gui elements, such as tabs or menus
/// </summary>
/// <param name="holdDownTime">The time it takes before the navigator progresses to the next element when a navigation key is held</param>
/// <param name="continuous">If the navigator should remember which index it was at after it has left the active selection</param>
/// <param name="looping">If the index should loop to the other side of the collection when reaching either end</param>
/// <param name="horizontal">Determines if the navigation keys are aligned horizontally or veritcally</param>
public abstract class Navigator(float holdDownTime, bool continuous = true, bool looping = true, bool horizontal = false) : SceneObject, ISectionable
{
    /// <summary>
    /// Invokes when any of the navigational keys parallell to the direction of the navigator are pressed
    /// at the end of the navigators span and it is not looping.
    /// Argument is false when that direction is left or up, otherwise its true.
    /// </summary>
    public event Action<bool>? NavigatorExit;
    /// <summary>
    /// Invokes when any of the navigational keys that are not parallell to the direction of the navigator are pressed.
    /// Argument is false when that direction is left or up, otherwise its true
    /// </summary>
    public event Action<bool>? OrthogonalExit; 
    private readonly Keyboard.Key[] increaseIndexKeys = horizontal ? [D, Right] : [S, Down];
    private readonly Keyboard.Key[] oIncreaseIndexKeys = !horizontal ? [D, Right] : [S, Down];
    private readonly Keyboard.Key[] decreaseIndexKeys = horizontal ? [A, Left] : [W, Up];
    private readonly Keyboard.Key[] oDecreaseIndexKeys = !horizontal ? [A, Left] : [W, Up];
    private readonly bool _looping = looping;
    private int _lastIndex;
    private int _pointerIndex;
    private readonly float _holdDown = holdDownTime;
    private float _holdDownTimer;
    private bool _keyPressed;
    private bool _firstFrame = true;
    private bool _inActiveSection = true;
    public override bool Active => base.Active && _inActiveSection;
    private int _exitIndex;

    private static readonly Sound _navigateSound = AssetManager.LoadSound("click2");
    
    protected abstract int Count();
    protected readonly bool _continuous = continuous;
    
    protected List<Keyboard.Key> _navigationalKeys
    {
        get
        {
            List<Keyboard.Key> r = new List<Keyboard.Key>();
            r.AddRange(increaseIndexKeys);
            r.AddRange(decreaseIndexKeys);
            r.AddRange(oIncreaseIndexKeys);
            r.AddRange(oDecreaseIndexKeys);
            return r;
        }
    }
    
    protected virtual List<Keyboard.Key> _allKeys
    {
        get
        {
            List<Keyboard.Key> r = new List<Keyboard.Key>();
            r.AddRange(_navigationalKeys);
            return r;
        }
    }


    public override void Update(float deltaTime)
    {
        if (_firstFrame && AreAnyKeysPressed(_allKeys.ToArray()))
        {
            return;
        }
        else _firstFrame = false;
        
        if (AreAnyKeysPressed(oIncreaseIndexKeys) && !_keyPressed) OrthogonalExit?.Invoke(true);
        if (AreAnyKeysPressed(oDecreaseIndexKeys) && !_keyPressed) OrthogonalExit?.Invoke(false);
        
        if (AreAnyKeysPressed(decreaseIndexKeys) && !_keyPressed)
        {
            // if we are on first element, invoke exit navigator if not looping
            if (_lastIndex == 0 && !_looping)
            {
                NavigatorExit?.Invoke(false);
                return;
            }
            HandleLastIndex(_lastIndex); // call deactivate for example
            
            _pointerIndex--;
            // adjust pointerIndex depening on looping
            if (_pointerIndex < 0) { _pointerIndex = _looping ? Count() - 1 : 0; }
            
            _navigateSound.Play();
            SelectNext(_pointerIndex);
            
            _lastIndex = _pointerIndex; // set new last index
            _keyPressed = true;
        }
        else if (AreAnyKeysPressed(increaseIndexKeys) && !_keyPressed)
        {
            // if we are on last element, dont do anything unless looping
            if (_lastIndex == Count() - 1 && !_looping)
            {
                NavigatorExit?.Invoke(true);
                return;
            }
            HandleLastIndex(_lastIndex); // call deactivate for example
            
            _pointerIndex++;
            // adjust pointerIndex depening on looping
            if (_pointerIndex >= Count()) { _pointerIndex = _looping ? 0 : Count() - 1; }
            
            _navigateSound.Play();
            SelectNext(_pointerIndex);
            
            _lastIndex = _pointerIndex; // set new last index
            _keyPressed = true;
        }
        else if (AreAnyKeysPressed(_navigationalKeys.ToArray()))
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
        
        if (!_firstFrame) NavigatorUpdate(deltaTime);
    }
    
    protected virtual void NavigatorUpdate(float deltaTime) { }
    protected abstract void SelectNext(int pointer);
    protected virtual void HandleLastIndex(int last) { }

    protected void PointerAction(Action<int> action)
    {
        action(_pointerIndex);
    }
    
    public virtual void SetActiveSelection()
    {
        _navigateSound.Play();
        SetIndex(_continuous ? _exitIndex : 0);
        _inActiveSection = true;
    }

    public virtual void SetInactiveSelection()
    {
        _exitIndex = _pointerIndex;
        _inActiveSection = false;
    }

    public override void Unpause()
    {
        base.Unpause();
        _firstFrame = true;
    }

    public virtual void SetIndex(int index)
    {
        _pointerIndex = index;
        
        if (index >= Count())
        {
            _pointerIndex = Count() - 1;
            _lastIndex = _pointerIndex;
        }

        if (index < 0)
        {
            _pointerIndex = 0;
            _lastIndex = _pointerIndex;
        }
    }

    public abstract void EnableNavigator();
    public abstract void DisableNavigator();
}