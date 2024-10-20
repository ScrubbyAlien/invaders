using System.Text.Json;

namespace invaders.saving;

public static class SaveManager
{
    private const string SavePath = "saves";
    
    private static string SaveFilePath(string name) => $"{Directory.GetCurrentDirectory()}/{SavePath}/{name}.save.json";
    
    // async callbacks learned from here
    // https://stackoverflow.com/questions/14455293/how-and-when-to-use-async-and-await
    public static async Task<T> LoadSave<T>() where T : ISaveObject, new()
    {
        T saveObject = new T();
        // if the file does not exist return false
        if (!File.Exists(SaveFilePath(saveObject.GetSaveFileName()))) return saveObject;
        
        // if it does exist, read it and return true
        string json = await File.ReadAllTextAsync(SaveFilePath(saveObject.GetSaveFileName()));
        return JsonSerializer.Deserialize<T>(json) ?? new T();
    }

    public static async Task WriteSave<T>(T save) where T : ISaveObject, new()
    {
        string json = JsonSerializer.Serialize(save);
        await File.WriteAllTextAsync(SaveFilePath(save.GetSaveFileName()), json);
    }   
}