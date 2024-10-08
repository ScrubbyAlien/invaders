using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public class Background : Entity
{
    private const float StarDensity = 0.0002f;
    private static readonly IntRect SmallStar = new(2, 54, 1, 1);
    private static readonly IntRect MediumStar = new(1, 57, 3, 3);
    private static readonly IntRect LargeStar = new(0, 62, 5, 5);
    private static readonly IntRect LargestStar = new(0, 69, 7, 7);

    private Dictionary<Vector2f, IntRect> StarMap =  new();
    private int _seed;
    private float _scroll;
    
    public Background(int seed) : base("invaders", SmallStar, 1)
    {
        GenerateStarMap();
        zIndex = -100;
        _seed = seed;
    }

    public Background() : base("invaders", SmallStar, 1)
    {
        GenerateStarMap();
        zIndex = -100;
    }

    public override void Update(float deltaTime)
    {
        _scroll += (Scene.AmbientScroll * deltaTime);
        _scroll %= Program.ScreenHeight;
    }

    public override void Render(RenderTarget target)
    {
        foreach (KeyValuePair<Vector2f,IntRect> star in StarMap)
        {
            float y = (star.Key.Y + _scroll) % Program.ScreenHeight;
            sprite.Position = new Vector2f(star.Key.X, y);
            sprite.TextureRect = star.Value;
            target.Draw(sprite);
        }
        
    }

    private void GenerateStarMap()
    {
        Random random = new Random();
        if (_seed != 0) random = new Random(_seed);
        StarMap = new();
        
        for (int i = 0; i < Program.ScreenHeight - LargestStar.Height; i++)
        {
            for (int j = 0; j < Program.ScreenWidth - LargestStar.Width; j++)
            {
                if (random.NextDouble() < StarDensity)
                {
                    StarMap[new Vector2f(j, i)] = random.Next(10) switch
                    {
                        0 => SmallStar,
                        1 => SmallStar,
                        2 => SmallStar,
                        3 => SmallStar,
                        4 => MediumStar,
                        5 => MediumStar,
                        6 => MediumStar,
                        7 => MediumStar,
                        8 => LargeStar,
                        9 => LargestStar,
                        _ => SmallStar
                    };
                }
            }
        }
    }
}