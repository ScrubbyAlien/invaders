using invaders.enums;
using invaders.sceneobjects;
using invaders.sceneobjects.gui;
using SFML.System;
using static invaders.Utility;

namespace invaders.levels;

public sealed class ScoreSaveLevel() : Level("scoresave")
{
    protected override void LoadObjects()
    {
        SetBackgroundMusic("finale");
        SetBackground();

        TextGUI message = new TextGUI("");
        message.AddTag(SceneObjectTag.Message);
        message.Position = MiddleOfScreen(message.Bounds, new Vector2f(0, -280));
        AddObject(message);

        TextInputGUI input = new TextInputGUI(10);
        input.PositionCalculator(o => MiddleOfScreen(o.Bounds, new Vector2f(0, 70)));
        AddObject(input);
        
        AddObject(new ScoreSaver());
    }
}