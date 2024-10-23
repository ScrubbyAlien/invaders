using invaders.sceneobjects;
using invaders.sceneobjects.renderobjects;
using invaders.sceneobjects.renderobjects.gui;

namespace invaders.levels;

public sealed class InvasionLevel() : Level("invasion")
{
    protected override void LoadObjects()
    {
        SetBackgroundMusic("invasion");

        AddObject(new Player());
        SetBackground();

        CreateTopGuiBackground();

        HealthGUI healthBar = new HealthGUI();
        healthBar.SetZIndex(310);
        AddObject(healthBar);

        AddObject(new ScoreManager());

        // Create WaveManager

        #region Assualts

        Assault assault1 = new Assault(["Incoming threat!", "First threat cleared"])
            .AddWave(new Wave(0f).Group('g', 10))
            .AddWave(new Wave(10f).Group('g', 10))
            .AddWave(new Wave(10f).Group('g', 7).Group('r', 3));

        Assault assault2 = new Assault(["More incoming!", "Second threat cleared"])
            .AddWave(new Wave(0f).Group('g', 8).Group('r', 1))
            .AddWave(new Wave(5f).Group('g', 8).Group('r', 1))
            .AddWave(new Wave(5f).Group('g', 8).Group('r', 1))
            .AddWave(new Wave(10f).Group('g', 8).Group('r', 2).Group('s', 2));

        Assault assault3 = new Assault(["Large group closing in!", "Third threat cleared"])
            .AddWave(new Wave(0f).Group('g', 15).Group('r', 3).Group('s', 2))
            .AddWave(new Wave(20f).Group('g', 15).Group('r', 3).Group('j', 1))
            .AddWave(new Wave(8f).Group('r', 5).Group('s', 2));

        Assault assault4 = new Assault(["They're coming fast!\nBe ready!", "Fourth threat cleared"])
            .AddWave(new Wave(0f).Group('g', 7).Group('r', 1))
            .AddWave(new Wave(3f).Group('g', 7).Group('r', 2))
            .AddWave(new Wave(3f).Group('g', 7).Group('r', 1))
            .AddWave(new Wave(3f).Group('g', 7).Group('r', 2).Group('s', 1))
            .AddWave(new Wave(3f).Group('g', 7).Group('r', 1).Group('s', 2))
            .AddWave(new Wave(3f).Group('g', 7).Group('r', 2).Group('s', 1).Group('j', 1));

        Assault assault5 = new Assault(["Even more on their way!", "Fifth threat cleared"])
            .AddWave(new Wave(0f).Group('g', 10).Group('r', 4).Group('s', 1))
            .AddWave(new Wave(12f).Group('g', 10).Group('r', 4).Group('j', 1))
            .AddWave(new Wave(12f).Group('g', 10).Group('r', 4).Group('s', 2))
            .AddWave(new Wave(12f).Group('g', 10).Group('s', 3).Group('r', 3).Group('j', 1));

        Assault assault6 = new Assault(["A whole bunch now!", "Sixth threat cleared"])
            .AddWave(new Wave(0f).Group('g', 15).Group('r', 7).Group('s', 4).Group('j', 1))
            .AddWave(new Wave(10f).Group('g', 15).Group('r', 7).Group('s', 4).Group('j', 1));

        Assault assault7 = new Assault(["This is the last of them!\nTake them out!", "Well done pilot!"])
            .AddWave(new Wave(0f).Group('g', 10).Group('r', 5).Group('s', 3))
            .AddWave(new Wave(15f).Group('g', 15).Group('r', 6).Group('s', 4).Group('j', 1))
            .AddWave(new Wave(15f).Group('g', 20).Group('r', 7).Group('s', 5).Group('j', 2))
            .AddWave(new Wave(15f).Group('g', 20).Group('r', 8).Group('s', 6).Group('j', 3));
        
        #endregion

        Assault[] assaults =
        [
            assault1,
            assault2,
            assault3,
            assault4,
            assault5,
            assault6,
            assault7
        ];

        WaveManager manager = new WaveManager();
        manager.AddAssault(assaults);
        AddObject(manager);

        CreatePauseMenu(Name);
    }
}