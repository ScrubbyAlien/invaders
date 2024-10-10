using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.entity;

public class Background : Entity
{
    private const float StarDensity = 0.0002f;

    private Dictionary<Vector2f, IntRect> StarMap =  new();
    private int _seed;
    private float _scroll;
    
    public Background(int seed) : base("invaders", TextureRects["smallStar"], 1)
    {
        GenerateStarMap();
        zIndex = -100;
        _seed = seed;
    }

    public Background() : base("invaders", TextureRects["smallStar"], 1)
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
        
        for (int i = 0; i < Program.ScreenHeight - TextureRects["largestStar"].Height; i++)
        {
            for (int j = 0; j < Program.ScreenWidth - TextureRects["largestStar"].Width; j++)
            {
                if (random.NextDouble() < StarDensity)
                {
                    StarMap[new Vector2f(j, i)] = random.Next(10) switch
                    {
                        0 => TextureRects["smallStar"],
                        1 => TextureRects["smallStar"],
                        2 => TextureRects["smallStar"],
                        3 => TextureRects["smallStar"],
                        4 => TextureRects["mediumStar"],
                        5 => TextureRects["mediumStar"],
                        6 => TextureRects["mediumStar"],
                        7 => TextureRects["mediumStar"],
                        8 => TextureRects["largeStar"],
                        9 => TextureRects["largestStar"],
                        _ => TextureRects["smallStar"]
                    };
                }
            }
        }
    }
}