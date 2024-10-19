using System.Reflection;
using invaders.sceneobjects;

namespace invaders;

public static class LevelManager
{
    private static List<Level> _levels = new();

    public static void Instantiate()
    {
        // get all classes in the current assembly
        // borrowed from example 1 https://www.iditect.com/program-example/c--how-to-get-all-classes-within-a-namespace.html
        Type[] levelTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "invaders.levels"))
            .ToArray();

        // runtime constructor calls borrowed from question at
        // https://stackoverflow.com/questions/5622519/how-to-create-an-instance-for-a-given-type
        foreach (Type level in levelTypes)
        {
            // <>c appears when lambda expressions exist in the class definition
            // it crashes if we try to call it's constructor so we ignore it
            if (level.Name == "<>c") continue; 
            Level? l = (Level?) level.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]);
            if (l != null) _levels.Add(l);
        }
    }
    
    public static List<SceneObject> LoadLevel(string name)
    {
        Level level = _levels.Where(level => level.Name == name).First();
        level.CreateLevel();
        return level.GetInitialObjects();
    }
}
