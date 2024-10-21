using invaders.enums;
using invaders.sceneobjects;
using invaders.sceneobjects.gui;
using static invaders.Utility;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace invaders.levels;

public sealed class EndlessLevel() : Level("endless")
{
    protected override void LoadObjects()
    {
        SetBackgroundMusic("invasion");

        Player player = new Player();
        AddObject(player);
        SetBackground();
        
        // create gui background
        SpriteGUI blackBar = new SpriteGUI(TextureRects["blackSquare"]);
        blackBar.Position = new Vector2f(0, 0);
        blackBar.SetScale(new Vector2f(Program.ScreenWidth, Settings.TopGuiHeight));
        blackBar.SetZIndex(290);
        AddObject(blackBar);
        
        SpriteGUI guiBackgroundMiddle = new SpriteGUI(TextureRects["guiBackgroundMiddle"]);
        guiBackgroundMiddle.Position = MiddleOfScreen(
            guiBackgroundMiddle.Bounds,
            new Vector2f(0, (-Program.ScreenHeight + guiBackgroundMiddle.Bounds.Height) / 2)
        );
        guiBackgroundMiddle.SetZIndex(300);
        AddObject(guiBackgroundMiddle);

        SpriteGUI guiBackgroundLeft = new SpriteGUI(TextureRects["guiBackgroundLeft"]);
        guiBackgroundLeft.Position = new Vector2f(0, 0);
        guiBackgroundLeft.SetZIndex(300);
        AddObject(guiBackgroundLeft);

        SpriteGUI guiBackgroundRight = new SpriteGUI(TextureRects["guiBackgroundRight"]);
        guiBackgroundRight.Position = new Vector2f(Program.ScreenWidth - guiBackgroundRight.Bounds.Width, 0);
        guiBackgroundRight.SetZIndex(300);
        AddObject(guiBackgroundRight);
        
        // create gui and score manager
        TextGUI scoreText = new TextGUI("0");
        scoreText.AddTag(SceneObjectTag.ScoreText);
        scoreText.SetZIndex(310);
        AddObject(scoreText);

        TextGUI multiplierText = new TextGUI("x1", 7);
        multiplierText.AddTag(SceneObjectTag.MultiplierText);
        multiplierText.SetZIndex(310);
        AddObject(multiplierText);
        
        SpriteGUI multiplierBar = new SpriteGUI(TextureRects["multiplierBar"]);
        multiplierBar.AddTag(SceneObjectTag.MultiplierBar);
        multiplierBar.SetScale(new Vector2f(100, 5));
        multiplierBar.SetZIndex(310);
        AddObject(multiplierBar);
        
        HealthGUI healthBar = new HealthGUI();
        healthBar.SetZIndex(310);
        AddObject(healthBar);
        
        AddObject(new ScoreManager(50, 1.3f));
        AddObject(new EndlessManager());
        
        // create pause menu and manager
        AddObject(new PauseManager(Keyboard.Key.Escape));
        ButtonNavigator pauseMenu = new ButtonNavigator();
        pauseMenu.AddTag(SceneObjectTag.PauseMenuItem);

        SpriteGUI transparentScreen = new SpriteGUI(TextureRects["blackSquare"]);
        transparentScreen.AddTag(SceneObjectTag.PauseMenuItem);
        transparentScreen.Position = new Vector2f(0, 0);
        transparentScreen.SetScale(new Vector2f(Program.ScreenWidth, Program.ScreenHeight));
        transparentScreen.SetColor(new Color(0, 0, 0, 170));
        transparentScreen.SetZIndex(1000);
        AddObject(transparentScreen);
        
        TextButtonGUI restartButton = new TextButtonGUI("restart");
        restartButton.AddTag(SceneObjectTag.PauseMenuItem);
        restartButton.Position = MiddleOfScreen(restartButton.Bounds, new Vector2f(0, -45));
        restartButton.SetZIndex(1100);
        AddObject(restartButton);
        
        TextButtonGUI quitButton = new TextButtonGUI("main menu");
        quitButton.AddTag(SceneObjectTag.PauseMenuItem);
        quitButton.Position = MiddleOfScreen(quitButton.Bounds, new Vector2f(0, 45));
        quitButton.SetZIndex(1100);
        AddObject(quitButton);
        
        pauseMenu.AddButton(restartButton, () =>
        {
            GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
            Scene.LoadLevel("endless");
            MusicManager.StopMusic();
        });
        pauseMenu.AddButton(quitButton, () =>
        {
            GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
            Scene.LoadLevel("mainmenu");
        });
        AddObject(pauseMenu);
    }
}