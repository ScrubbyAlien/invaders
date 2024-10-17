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
        
        
        // create gui and score manager
        TextGUI scoreText = new TextGUI("x1\n0", TextGUI.Alignment.Right);
        scoreText.SetTag(SceneObjectTag.ScoreText);
        scoreText.Position = BottomRightOfScreen(scoreText.Bounds, new Vector2f(-24, -24));
        AddObject(scoreText);

        SpriteGUI multiplierBar = new SpriteGUI(TextureRects["multiplierBar"]);
        multiplierBar.SetTag(SceneObjectTag.MultiplierBar);
        multiplierBar.SetScale(new Vector2f(100, 5));
        multiplierBar.Position = BottomRightOfScreen(
            multiplierBar.Bounds,
            new Vector2f(-24, -24 - scoreText.Bounds.Height - 8));
        AddObject(multiplierBar);
        
        AddObject(new ScoreManager());
        AddObject(new HealthGUI());
    }
}