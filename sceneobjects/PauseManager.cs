using invaders.enums;
using invaders.sceneobjects.renderobjects;
using SFML.Window;
using static invaders.Utility;

namespace invaders.sceneobjects;

public sealed class PauseManager(Keyboard.Key pauseKey) : SceneObject
{
    private bool _pressedPause;
    private bool _isPaused;
    private Keyboard.Key _pauseKey = pauseKey;

    public override bool Active
    {
        get => true; // you can't pause the pause manager
    }

    protected override void Initialize()
    {
        foreach (RenderObject r in Scene.FindAllByTag<RenderObject>(SceneObjectTag.PauseMenuItem))
        {
            r.Hide();
            r.Pause();
        }
    }

    public override void Update(float deltaTime)
    {
        if (AreAnyKeysPressed([_pauseKey]) && !_pressedPause)
        {
            _pressedPause = true;
            if (_isPaused) UnpauseScene();
            else PauseScene();
        }
        // prevents rapid pausing and unpausing if the pause key is held
        if (!AreAnyKeysPressed([_pauseKey])) _pressedPause = false; 
    }

    private void PauseScene()
    {
        foreach (SceneObject o in Scene.FindAllByType<SceneObject>())
        {
            if (!o.HasTag(SceneObjectTag.PauseMenuItem)) o.Pause();
            else 
            {
                o.Unpause();
                if (o is RenderObject r) r.Unhide(); 
            }
        }

        _isPaused = true;
    }

    private void UnpauseScene()
    {
        foreach (SceneObject o in Scene.FindAllByType<SceneObject>())
        {
            if (!o.HasTag(SceneObjectTag.PauseMenuItem)) o.Unpause();
            else 
            {
                o.Pause();
                if (o is RenderObject r) r.Hide(); 
            }
        }

        _isPaused = false;
    }
}