using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public sealed class Background : RenderObject
{
    private const float StarDensity = 0.0002f;

    private Dictionary<Vector2f, string> StarMap =  new();
    private int _seed;
    private float _scroll;
    private float _scrollSpeed = Settings.AmbientScrollInLevel;
    private float _targetScrollSpeed = Settings.AmbientScrollInLevel;
    private float _originalScrollSpeed = Settings.AmbientScrollInLevel;
    private float _lerpTime;
    private float _lerpTimer;
    private bool _lerping;
    public float ScrollSpeed => _scrollSpeed;
    
    public Background() : base("invaders", TextureRects["smallStar"], 1)
    {
        GenerateStarMap();
        zIndex = -100;
        DontDestroyOnClear = true;
    }
    public Background(int seed) : this() { _seed = seed; }
    public Background(int seed, float scroll) : this(seed) { _scroll = scroll; }

    protected override void Initialize()
    {
        GlobalEventManager.BackgroundSetScrollSpeed += SetNewScrollSpeed;
    }

    public override void Destroy()
    {
        GlobalEventManager.BackgroundSetScrollSpeed -= SetNewScrollSpeed;
    }

    public override void Update(float deltaTime)
    {
        if (_lerping)
        {
            _lerpTimer += deltaTime;
            float t = _lerpTimer / _lerpTime;
            _scrollSpeed = float.Lerp(_originalScrollSpeed, _targetScrollSpeed, t);
            if (_lerpTimer >= _lerpTime)
            {
                _scrollSpeed = _targetScrollSpeed;
                _lerping = false;
            }
        }
        
        _scroll += _scrollSpeed * deltaTime;
        _scroll %= Program.ScreenHeight;
    }

    public override void Render(RenderTarget target)
    {
        foreach (KeyValuePair<Vector2f,string> star in StarMap)
        {
            float y = (star.Key.Y + _scroll) % Program.ScreenHeight;
            sprite.Position = new Vector2f(star.Key.X, y);
            bool middleStar = _scrollSpeed > 1000;
            bool longStar = _scrollSpeed > 2000;
            string starString = star.Value;
            if (middleStar) starString = star.Value + "Middle";
            if (longStar) starString = star.Value + "Long";
            sprite.TextureRect = TextureRects[starString];
            sprite.Position += new Vector2f(0, -Bounds.Height - 2);
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
                        0 => "smallStar",
                        1 => "smallStar",
                        2 => "smallStar",
                        3 => "smallStar",
                        4 => "mediumStar",
                        5 => "mediumStar",
                        6 => "mediumStar",
                        7 => "mediumStar",
                        8 => "largeStar",
                        9 => "largestStar",
                        _ => "smallStar"
                    };
                }
            }
        }
    }

    private void SetNewScrollSpeed(float speed, float lerpTime)
    {
        if (Math.Abs(_targetScrollSpeed - speed) > float.Epsilon)
        {
            _originalScrollSpeed = _scrollSpeed;
            _targetScrollSpeed = speed;
            _lerpTime = lerpTime;
            _lerpTimer = 0f;
            _lerping = true;
        }
    }
}