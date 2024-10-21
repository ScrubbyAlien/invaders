using invaders.enums;
using SFML.System;
using static invaders.Utility;
using invaders.saving;
using invaders.sceneobjects.gui;

namespace invaders.sceneobjects;

public sealed class ScoreSaver : SceneObject
{
    private int _score;
    private bool _won;
    private bool _endless;
    private TextGUI _message = null!;
    
    protected override void Initialize()
    {
        _score = LevelInfo<int>.Catch("score", 0);
        _won = LevelInfo<bool>.Catch("won", false);
        _endless = LevelInfo<bool>.Catch("endless", false);
        
        _message = Scene.FindByTag<TextGUI>(SceneObjectTag.Message)!;
        if (Scene.FindByType(out TextInputGUI? input))
        {
            input!.InputEntered += SaveScore;
        }

        if (_won)
        {
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
                             "Valiant effort pilot!\n" +
                             "What is your name?");
            _message.Position = MiddleOfScreen(_message.Bounds, new Vector2f(0, -140));

        }
    }

    private void SaveScore(object? _, string name)
    {
        ScoresSaveObject scores = SaveManager.LoadSave<ScoresSaveObject>().Result; // load existing save
        
        if (scores.AddEntry(name.ToLower(), _score, _endless)) // write new value
        {
            SaveManager.WriteSave(scores).Wait(); // write new save, overwriting old
            Scene.QueueSpawn(new LevelInfo<bool>(_endless, "endless"));
            Scene.LoadLevel("highscores"); // load high score screen
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