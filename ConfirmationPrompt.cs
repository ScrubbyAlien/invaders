using invaders.enums;
using invaders.sceneobjects;
using invaders.sceneobjects.gui;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders;

public class ConfirmationPrompt
{
    
    private ButtonNavigator _yesOrNoMenu = new ButtonNavigator(true, true);
    private TextButtonGUI _yesButton = new TextButtonGUI("yes");
    private TextButtonGUI _noButton = new TextButtonGUI("no");
    private List<SceneObject> _prompt = new();

    public ConfirmationPrompt(string message, Action yes, Action no)
    {
        TextGUI messageText = new TextGUI(message);
        messageText.SetZIndex(1100);
        messageText.Position = MiddleOfScreen(messageText.Bounds, new Vector2f(0, -100));

        SpriteGUI background = new SpriteGUI(TextureRects["blackSquare"]);
        background.SetScale(new Vector2f(Program.ScreenWidth, Program.ScreenHeight));
        background.SetZIndex(1000);
        background.SetColor(new Color(0, 0, 0, 200));

        _yesButton.Position = new Vector2f(
            Program.ScreenWidth / 2f - _yesButton.Bounds.Width - Settings.MarginSide,
            messageText.Position.Y + messageText.Bounds.Height + Settings.MarginSide * 3
        );
        _yesButton.SetZIndex(1100);

        _noButton.Position = new Vector2f(
            Program.ScreenWidth / 2f + _noButton.Bounds.Width + Settings.MarginSide,
            messageText.Position.Y + messageText.Bounds.Height + Settings.MarginSide * 3
        );
        _noButton.SetZIndex(1100);

        _yesOrNoMenu.AddButton(_yesButton, () =>
        {
            ClosePrompt();
            yes();
        });
        _yesOrNoMenu.AddButton(_noButton, () =>
        {
            ClosePrompt();
            no();
        });

        _prompt.Add(background);
        _prompt.Add(messageText);
        _prompt.Add(_yesButton);
        _prompt.Add(_noButton);
        _prompt.Add(_yesOrNoMenu);
    }

    private void ClosePrompt()
    {
        Scene.FindAllByType<SceneObject>().ForEach(o =>
        {
            if (!o.HasTag(SceneObjectTag.PauseMenuItem)) o.Unpause();
        });
        Scene.QueueDestroy(_prompt);
    }

    public void Prompt()
    {
        Scene.FindAllByType<SceneObject>().ForEach(o => o.Pause());
        Scene.QueueSpawn(_prompt);
    }
}