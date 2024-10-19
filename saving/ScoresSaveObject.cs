using invaders.interfaces;

namespace invaders.saving;

public class ScoresSaveObject : ISaveObject
{
    public string GetSaveFileName() => "scores";
    
    private Dictionary<string, int> _scores = new();
    private Dictionary<string, int> _endlessScores = new();
    public Dictionary<string, int> Scores
    {
        get => _scores;
        set => _scores = value;
    }

    public Dictionary<string, int> EndlessScores
    {
        get => _endlessScores;
        set => _endlessScores = value;
    }

    public bool AddEntry(string key, int value, bool endless = false)
    {
        if (endless)
        {
            if (_endlessScores.ContainsKey(key)) return false;
            _endlessScores.Add(key, value);
            return true;
        }
        if (_scores.ContainsKey(key)) return false;
        _scores.Add(key, value);
        return true;
    }

    public int ReadEntry(string key, bool endless = false)
    {
        if (endless) return _endlessScores[key];
        return _scores[key];
    }
}