using invaders.sceneobjects.renderobjects;
using invaders.sceneobjects.renderobjects.gui;
using static invaders.Utility;
using SFML.System;
using SFML.Window;

namespace invaders.sceneobjects;

public abstract class Invasion : SceneObject
{
    protected bool inEndLevel;
    protected readonly TextGUI messageText = new("");
    private bool _spaceReleased;
    public virtual bool InTransition => inEndLevel;

    // entity constructor dictionary system borrowed from lab project 4
    protected static readonly Dictionary<char, Func<AbstractEnemy>> Constructors = new() {
        { 'g', () => new Grunt() },
        { 'r', () => new Runner() },
        { 's', () => new Squid() },
        { 'j', () => new Juggernaut() },
    };

    protected override void Initialize() {
        messageText.SetSize(10);
        messageText.GetAnimatable().Animator.PlayAnimation("blink", true);
        messageText.Position = MiddleOfScreen(messageText.Bounds, new Vector2f(0, -100));
        Scene.QueueSpawn(messageText);
        GlobalEventManager.PlayerDeath += PlayerDied;
    }

    public override void Destroy() => GlobalEventManager.PlayerDeath -= PlayerDied;

    public override void Update(float deltaTime) {
        if (!inEndLevel) return;

        if (AreAnyKeysPressed([Keyboard.Key.Space])) {
            if (!_spaceReleased) return;
            GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);

            Scene.LoadLevel("scoresave");
        }
        else {
            _spaceReleased = true;
        }
    }

    private void PlayerDied() {
        inEndLevel = true;

        DrawText(
            "You have been defeated!\n" +
            " \n" +
            "press space to continue",
            new Vector2f(0, -100));
    }

    protected void DrawText(string text, Vector2f positionFromMiddle) {
        messageText.Unhide();
        messageText.SetText(text);
        messageText.Position = MiddleOfScreen(messageText.Bounds) + positionFromMiddle;
        messageText.GetAnimatable().Animator.PlayAnimation("blink", false);
    }

    protected void HideText() {
        messageText.Hide();
        messageText.GetAnimatable().Animator.StopAnimation();
    }
}