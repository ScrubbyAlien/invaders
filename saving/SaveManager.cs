using System.Text.Json;
using invaders.interfaces;

namespace invaders.saving;

public static class SaveManager
{
    private const string SavePath = "saves";
    
    private static string SaveFilePath(string name) => $"{Directory.GetCurrentDirectory()}/{SavePath}/{name}.save.json";
    
    public static void LoadSave<T>(ref T saveObject) where T : ISaveObject, new()
    {
        // if the file does not exist return false
        if (!File.Exists(SaveFilePath(saveObject.GetSaveFileName()))) return;
        
        // if it does exist, read it and return true
        string json = File.ReadAllText(SaveFilePath(saveObject.GetSaveFileName()));
        saveObject = JsonSerializer.Deserialize<T>(json) ?? new T();
    }

    public static void WriteSave<T>(T save) where T : ISaveObject, new()
    {
        string json = JsonSerializer.Serialize(save);
        File.WriteAllText(SaveFilePath(save.GetSaveFileName()), json);
    }   
}