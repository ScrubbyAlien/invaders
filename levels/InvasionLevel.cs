using invaders.sceneobjects;

namespace invaders.levels;

public class InvasionLevel() : Level("invasion")
{
    public override void CreateLevel()
    {
        AddInitialObject(new Player());
        if (!Scene.FindByType(out Background _))
        {
            AddInitialObject(new Background());
        }
        AddInitialObject(new WaveManager());
        AddInitialObject(new HealthGUI());
    }
}