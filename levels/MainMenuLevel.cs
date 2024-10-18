using invaders.interfaces;
using invaders.sceneobjects;
using SFML.System;
using static invaders.Utility;

namespace invaders.levels;

public class MainMenuLevel() : Level("mainmenu")
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
        
        TextButtonGUI playButton = new TextButtonGUI("Play");
        playButton.Position = MiddleOfScreen(playButton.Bounds, new Vector2f(0, 70));
        TextButtonGUI highScoreButton = new TextButtonGUI("Highscores");
        highScoreButton.Position = MiddleOfScreen(highScoreButton.Bounds, new Vector2f(0, 140));
        TextButtonGUI quitButton = new TextButtonGUI("Quit");
        quitButton.Position = MiddleOfScreen(quitButton.Bounds, new Vector2f(0, 210));
        AddObject(playButton);
        AddObject(highScoreButton);
        AddObject(quitButton);

        MenuManager manager = new MenuManager();
        manager.AddButton(playButton, Scene.LoadLevelListener("gamemode"));
        manager.AddButton(quitButton, Scene.CloseWindowListener());
        AddObject(manager);
    }
}