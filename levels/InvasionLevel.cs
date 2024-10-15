using invaders.sceneobjects;

namespace invaders.levels;

public class InvasionLevel() : Level("invasion")
{
    public override void CreateLevel()
    {
        AddInitialObject(new Player());
        AddInitialObject(new Background()); // todo: check if background seed already exists
        AddInitialObject(new WaveManager());
        AddInitialObject(new HealthGUI());
    }
}