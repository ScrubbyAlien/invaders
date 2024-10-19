using invaders.enums;
using SFML.System;
using static invaders.Utility;
using invaders.saving;

namespace invaders.sceneobjects;

public sealed class ScoreSaver : SceneObject
{
    private int _score = 1000;
    private bool _won = true;
    private TextGUI _message = null!;
    private TextInputGUI _input = null!;
    
    protected override void Initialize()
    {
        if (Scene.FindByType(out LevelInfo<int> finalScore)) _score = finalScore.Extract();
        if (Scene.FindByType(out LevelInfo<bool> won)) _won = won.Extract();
        _message = Scene.FindByTag<TextGUI>(SceneObjectTag.Message);
        _input = Scene.FindByType<TextInputGUI>();
        _input.InputEntered += SaveScore;

        if (_won)
        {
            _input.Paused = false;
            _input.Unhide();
            _message.SetText("Final score:\n" +
                             $"{_score}\n" +
                             " \n" +
                             "Well done pilot!\n" +
                             "What is your name?");
            _message.Position = MiddleOfScreen(_message.Bounds, new Vector2f(0, -140));

        }
        else
        {
            _message.SetText("Final score:\n" +
                             $"{_score}\n" +
                             " \n" +
                             "The invaders won\n" +
                             "Earth is lost");
            _message.Position = MiddleOfScreen(_message.Bounds, new Vector2f(0, -140));

        }
    }

    private void SaveScore(object? _, string name)
    {
        if (!_won) // don't save score when you lost
        {
            Scene.LoadLevel("mainmenu");
            return;
        }

        ScoresSaveObject scores = new ScoresSaveObject();
        SaveManager.LoadSave(ref scores); // load existing save
        if (scores.AddEntry(name.ToLower(), _score)) // write new value
        {
            SaveManager.WriteSave(scores); // write new save, overwriting old
            Scene.LoadLevel("mainmenu"); // load high score screen
            return;
        }
        
        // show warning if entry already exists
        
        // remove any preexisting warnings
        Scene.FindAllByType<FadingTextGUI>().ForEach(o => Scene.QueueDestroy(o));
        
        FadingTextGUI fadingWarning = new FadingTextGUI(2f, "Another pilot with that\nname already exists", 30);
        fadingWarning.Position = MiddleOfScreen(fadingWarning.Bounds, new Vector2f(0, 170));
        fadingWarning.SetDrift(new Vector2f(0, 1) * 10);
        Scene.QueueSpawn(fadingWarning);
    }
}