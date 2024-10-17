using invaders.enums;
using invaders.sceneobjects;
using SFML.Graphics;
using SFML.System;
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

        SpriteGUI guiBackgroundMiddle = new SpriteGUI(TextureRects["guiBackgroundMiddle"]);
        guiBackgroundMiddle.Position = MiddleOfScreen(
            guiBackgroundMiddle.Bounds,
            new Vector2f(0, (-Program.ScreenHeight + guiBackgroundMiddle.Bounds.Height) / 2)
        );
        guiBackgroundMiddle.SetZIndex(300);
        AddObject(guiBackgroundMiddle);
        
        
        
        SpriteGUI blackBar = new SpriteGUI(TextureRects["blackSquare"]);
        blackBar.Position = new Vector2f(0, 0);
        blackBar.SetScale(new Vector2f(Program.ScreenWidth, Settings.TopGuiHeight));
        blackBar.SetZIndex(290)
            ;
        AddObject(blackBar);

        

        // Create WaveManager
        string[] assault1Strings = ["Incoming threat!\nTest", "First threat cleared"];
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
        
        
        // create gui and score manager
        TextGUI scoreText = new TextGUI("0", 10, TextGUI.Alignment.Right);
        scoreText.SetTag(SceneObjectTag.ScoreText);
        scoreText.SetZIndex(310);
        AddObject(scoreText);

        SpriteGUI multiplierBar = new SpriteGUI(TextureRects["multiplierBar"]);
        multiplierBar.SetTag(SceneObjectTag.MultiplierBar);
        multiplierBar.SetScale(new Vector2f(100, 5));
        AddObject(multiplierBar);
        
        AddObject(new ScoreManager(100, 1.7f));
        AddObject(new HealthGUI());
    }
}