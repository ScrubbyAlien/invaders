using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace invaders;

// SFML main loop implementation borrowed wholesale from lab project 3 and 4
internal static class Program
{
    public const int ScreenWidth = 720;
    public const int ScreenHeight = 900;

    public static void Main(string[] args) {
        using (RenderWindow window = new(new VideoMode(ScreenWidth, ScreenHeight), "Invaders")) {
            // ReSharper disable once AccessToDisposedClosure
            window.Closed += (_, _) => window.Close();
            window.SetFramerateLimit(Settings.FrameLimit);

            Clock clock = new();
            LevelManager.Instantiate();
            Scene.SetWindow(window);
            Scene.LoadFirstLevel();

            while (window.IsOpen) {
                window.DispatchEvents();
                float deltaTime = clock.Restart().AsSeconds();
                Scene.UpdateAll(deltaTime);

                window.Clear();
                Scene.RenderAll(window);

                window.Display();
            }
        }
    }
}