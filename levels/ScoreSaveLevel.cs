using invaders.enums;
using invaders.sceneobjects;
using SFML.System;
using static invaders.Utility;

namespace invaders.levels;

public sealed class ScoreSaveLevel() : Level("scoresave")
{
    protected override void LoadObjects()
    {
        if (!Scene.FindByType(out Background _))
        {
            AddObject(new Background());
        }

        TextGUI message = new TextGUI("");
        message.AddTag(SceneObjectTag.Message);
        message.Position = MiddleOfScreen(message.Bounds, new Vector2f(0, -280));
        AddObject(message);

        TextInputGUI input = new TextInputGUI(10);
        input.PositionCalculator(o => MiddleOfScreen(o.Bounds, new Vector2f(0, 70)));
        input.Pause();
        input.Hide();
        AddObject(input);
        
        AddObject(new ScoreSaver());
    }
}