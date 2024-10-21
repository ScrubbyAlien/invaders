using invaders.sceneobjects;
using invaders.sceneobjects.gui;
using SFML.System;
using static invaders.Utility;

namespace invaders.levels;

public sealed class MainMenuLevel() : Level("mainmenu")
{
    protected override void LoadObjects()
    {
        SetBackgroundMusic("mainmenu");
        
        SetBackground();

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

        ButtonNavigator manager = new ButtonNavigator();
        manager.AddButton(playButton, () => Scene.LoadLevel("gamemodeselect"));
        manager.AddButton(highScoreButton, () => Scene.LoadLevel("highscores"));
        manager.AddButton(quitButton, () => Scene.CloseWindow());
        AddObject(manager);
    }
}