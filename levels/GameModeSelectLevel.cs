using SFML.System;
using invaders.sceneobjects;
using static invaders.Utility;

namespace invaders.levels;

public sealed class GameModeSelectLevel() : Level("gamemode")
{
    protected override void LoadObjects()
    {
        if (!Scene.FindByType(out Background _))
        {
            AddObject(new Background());
        }

        SpriteGUI title = new SpriteGUI(TextureRects["title"]);
        title.SetScale(10);
        title.Position = MiddleOfScreen(title.Bounds, new Vector2f(0, -200));
        AddObject(title);
        
        TextButtonGUI standardButton = new TextButtonGUI("standard");
        standardButton.Position = MiddleOfScreen(standardButton.Bounds, new Vector2f(0, 70));
        TextButtonGUI endlessButton = new TextButtonGUI("endless");
        endlessButton.Position = MiddleOfScreen(endlessButton.Bounds, new Vector2f(0, 140));
        TextButtonGUI backButton = new TextButtonGUI("back");
        backButton.Position = MiddleOfScreen(backButton.Bounds, new Vector2f(0, 210));
        
        AddObject(standardButton);
        AddObject(endlessButton);
        AddObject(backButton);

        MenuManager manager = new MenuManager();
        manager.AddButton(standardButton, Scene.LoadLevelListener("invasion"));
        manager.AddButton(backButton, Scene.LoadLevelListener("mainmenu"));
        AddObject(manager);
    }
}