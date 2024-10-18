using invaders.enums;
using invaders.sceneobjects;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static invaders.Utility;

namespace invaders.levels;

public class InvasionLevel() : Level("invasion")
{
    protected override void LoadObjects()
    {
        AddObject(new Player());
        if (!Scene.FindByType(out Background _))
        {
            AddObject(new Background());
        }

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
        
        AddObject(new ScoreManager());
        
        // Create WaveManager
        string[] assault1Strings = ["Incoming threat!", "First threat cleared"];
        Assault assault1 = new Assault(assault1Strings);
        assault1.AddWave(new Wave(0f).AddEnemyGroup('g', 7));
        assault1.AddWave(new Wave(10f).AddEnemyGroup('g', 10));
        assault1.AddWave(new Wave(10f).AddEnemyGroup('g', 10));
        
        string[] assault2Strings = ["More incoming!", "Second threat cleared"];
        Assault assault2 = new Assault(assault2Strings);
        assault2.AddWave(new Wave(0f).AddEnemyGroup('g', 7));
        assault2.AddWave(new Wave(10f).AddEnemyGroup('g', 10));
        assault2.AddWave(new Wave(10f).AddEnemyGroup('g', 10));

        WaveManager manager = new WaveManager();
        manager.AddAssault([assault1, assault2]);
        AddObject(manager);
        
        // create pause menu and manager
        AddObject(new PauseManager(Keyboard.Key.Escape));
        MenuManager pauseMenu = new MenuManager();
        pauseMenu.AddTag(SceneObjectTag.PauseMenuItem);

        SpriteGUI transparentScreen = new SpriteGUI(TextureRects["blackSquare"]);
        transparentScreen.AddTag(SceneObjectTag.PauseMenuItem);
        transparentScreen.Position = new Vector2f(0, 0);
        transparentScreen.SetScale(new Vector2f(Program.ScreenWidth, Program.ScreenHeight));
        transparentScreen.SetColor(new Color(0, 0, 0, 100));
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
        
        pauseMenu.AddButton(restartButton, Scene.LoadLevelListener("invasion"));
        pauseMenu.AddButton(quitButton, Scene.LoadLevelListener("mainmenu"));
        AddObject(pauseMenu);
    }
}