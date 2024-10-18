using invaders.enums;
using SFML.Window;
using static invaders.Utility;

namespace invaders.sceneobjects;

public class PauseManager(Keyboard.Key pauseKey) : SceneObject
{
    private bool _pressedPause;
    private bool _isPaused;
    private Keyboard.Key _pauseKey = pauseKey;

    public override bool Paused
    {
        get => false; // you can't pause the pause manager
    }

    protected override void Initialize()
    {
        foreach (RenderObject r in Scene.FindAllByTag<RenderObject>(SceneObjectTag.PauseMenuItem))
        {
            r.Hide();
        }
    }

    public override void Update(float deltaTime)
    {
        if (AreAnyKeysPressed([_pauseKey]) && !_pressedPause)
        {
            _pressedPause = true;
            if (_isPaused) Unpause();
            else Pause();
        }
        // prevents rapid pausing and unpausing if the pause key is held
        if (!AreAnyKeysPressed([_pauseKey])) _pressedPause = false; 
    }

    private void Pause()
    {
        foreach (SceneObject o in Scene.FindAllByType<SceneObject>())
        {
            if (!o.HasTag(SceneObjectTag.PauseMenuItem)) o.Paused = true;
            else 
            {
                o.Paused = false;
                if (o is RenderObject r) r.Unhide(); 
            }
        }

        _isPaused = true;
    }

    private void Unpause()
    {
        foreach (SceneObject o in Scene.FindAllByType<SceneObject>())
        {
            if (!o.HasTag(SceneObjectTag.PauseMenuItem)) o.Paused = false;
            else 
            {
                o.Paused = true;
                if (o is RenderObject r) r.Hide(); 
            }
        }

        _isPaused = false;
    }
}