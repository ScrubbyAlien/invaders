using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace invaders;

internal static class Program
{
    public const int ScreenWidth = 720;
    public const int ScreenHeight = 900;
    
    public static void Main(string[] args)
    {
        using (RenderWindow window = new RenderWindow(new VideoMode(ScreenWidth, ScreenHeight), "Invaders"))
        {
            // ReSharper disable once AccessToDisposedClosure
            window.Closed += (o, e) => window.Close();

            Clock clock = new();
            Scene scene = new();

            while (window.IsOpen)
            {
                window.DispatchEvents();
                float deltaTime = clock.Restart().AsSeconds();
                scene.UpdateAll(deltaTime);
                
                window.Clear();
                scene.RenderAll(window);
                
                window.Display();
            }
        }
    }
}