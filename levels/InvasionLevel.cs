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
        AddObject(new WaveManager());
        AddObject(new HealthGUI());
    }
}