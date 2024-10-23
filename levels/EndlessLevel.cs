using invaders.sceneobjects;
using invaders.sceneobjects.renderobjects;
using invaders.sceneobjects.renderobjects.gui;

namespace invaders.levels;

public sealed class EndlessLevel() : Level("endless")
{
    protected override void LoadObjects() {
        SetBackgroundMusic("invasion");

        Player player = new();
        AddObject(player);
        SetBackground();

        CreateTopGuiBackground();

        HealthGUI healthBar = new();
        healthBar.SetZIndex(310);
        AddObject(healthBar);

        AddObject(new ScoreManager(50, 1.3f));
        AddObject(new EndlessManager());

        CreatePauseMenu(Name);
    }
}