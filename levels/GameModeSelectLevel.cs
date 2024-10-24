using SFML.System;
using invaders.sceneobjects.renderobjects.gui;
using static invaders.Utility;

namespace invaders.levels;

public sealed class GameModeSelectLevel() : Level("gamemodeselect")
{
    protected override void LoadObjects() {
        SetBackgroundMusic("mainmenu");

        SetBackground();

        SpriteGUI title = new(TextureRects["title"]);
        title.SetScale(10);
        title.Position = MiddleOfScreen(title.Bounds, new Vector2f(0, -200));
        AddObject(title);

        TextButtonGUI standardButton = new("standard");
        standardButton.Position = MiddleOfScreen(standardButton.Bounds, new Vector2f(0, 70));
        TextButtonGUI endlessButton = new("endless");
        endlessButton.Position = MiddleOfScreen(endlessButton.Bounds, new Vector2f(0, 140));
        TextButtonGUI backButton = new("back");
        backButton.Position = MiddleOfScreen(backButton.Bounds, new Vector2f(0, 210));

        AddObject(standardButton);
        AddObject(endlessButton);
        AddObject(backButton);

        ButtonNavigator manager = new();
        manager.AddButton(standardButton, () => Scene.LoadLevel("invasion"));
        manager.AddButton(endlessButton, () => Scene.LoadLevel("endless"));
        manager.AddButton(backButton, () => Scene.LoadLevel("mainmenu"));
        AddObject(manager);
    }
}