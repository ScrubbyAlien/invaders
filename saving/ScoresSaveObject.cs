namespace invaders.saving;

public class ScoresSaveObject : ISaveObject
{
    public string GetSaveFileName() => "scores";

    public Dictionary<string, int> Scores { get; } = new();
    public Dictionary<string, int> EndlessScores { get; } = new();

    public bool AddEntry(string key, int value, bool endless = false)
    {
        if (endless)
        {
            if (EndlessScores.ContainsKey(key)) return false;
            EndlessScores.Add(key, value);
            return true;
        }
        if (Scores.ContainsKey(key)) return false;
        Scores.Add(key, value);
        return true;
    }
}