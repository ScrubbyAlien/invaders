using invaders.sceneobjects;

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
        AddObject(new HealthGUI());
    }
}