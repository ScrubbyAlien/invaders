using System.Reflection;
using invaders.sceneobjects;

namespace invaders;

public static class LevelManager
{
    private static readonly List<Level> _levels = new();

    public static void Instantiate() {
        // get all classes in the current assembly
        // borrowed from example 1 https://www.iditect.com/program-example/c--how-to-get-all-classes-within-a-namespace.html
        Type[] levelTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => string.Equals(t.Namespace, "invaders.levels"))
            .ToArray();

        // runtime constructor calls borrowed from question at
        // https://stackoverflow.com/questions/5622519/how-to-create-an-instance-for-a-given-type
        // create instances of all classes of type Level within namespace levels, and add to _levels
        foreach (Type level in levelTypes) {
            // extra types appear when lambda expressions exist in the class definition
            // there are a few versions of these types and none of them inheritly cast to Level
            // all of them seem to include the char '<' so we ignore those, that way we doesn't crash when we cast to Level
            if (level.Name.Contains('<')) continue;
            Level? l = (Level?)level.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
            if (l != null) _levels.Add(l);
        }
    }

    public static List<SceneObject> LoadLevel(string name) {
        Level level = _levels.Where(level => level.Name == name).First();
        level.CreateLevel();
        return level.GetInitialObjects();
    }
}