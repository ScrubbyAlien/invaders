using invaders.sceneobjects.gui;
using static invaders.Utility;
using SFML.System;
using SFML.Window;

namespace invaders.sceneobjects;

public abstract class Invasion : SceneObject
{
    protected bool inEndLevel;
    protected TextGUI _messageText = new("");
    private bool _spaceReleased;
    public virtual bool InTransition => inEndLevel;
    
    // entity constructor dictionary system borrowed from lab project 4
    public static readonly SortedDictionary<char, Func<AbstractEnemy>> Constructors = new()
    {
        { 'g', () => new Grunt() },
    };
    
    
    protected override void Initialize()
    {
        GlobalEventManager.PlayerDeath += PlayerDied;
    }

    public override void Destroy()
    {
        GlobalEventManager.PlayerDeath -= PlayerDied;
    }

    public override void Update(float deltaTime)
    {
        if (inEndLevel)
        {
            if (AreAnyKeysPressed([Keyboard.Key.Space]))
            {
                if (!_spaceReleased) return;
                GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
                
                Scene.LoadLevel("scoresave");
            }
            else _spaceReleased = true;
        }
    }

    protected void PlayerDied()
    {
        inEndLevel = true; 
        
        DrawText(
            "You have been defeated!\n" +
            " \n" +
            "press space to continue",
            new Vector2f(0, -100));
    }
    
    protected void DrawText(string text, Vector2f positionFromMiddle)
    {
        _messageText = new TextGUI(text, 8);
        _messageText.Position = MiddleOfScreen(_messageText.Bounds) + positionFromMiddle;
        Scene.QueueSpawn(_messageText);
        // call PlayAnimatio after next ProcessSpawnQueue call so _messageText's Initialize method can be called first
        Scene.DeferredCall(_messageText.GetAnimatable().Animator, "PlayAnimation", ["blink", true]);
    }
}