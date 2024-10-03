using SFML.Graphics;
using SFML.System;

namespace invaders;

public abstract class Entity
{
    protected Sprite sprite = new();
    protected string textureName;
    public const int SpriteWidth = 0;
    public const int SpriteHeight = 0;
    public const float Scale = 1f;
    public bool Dead;
    public bool DontDestroyOnLoad;

    public Entity(string textureName, IntRect initRect, float scale)
    {
        this.textureName = textureName;
        AssetManager.LoadTexture(textureName, initRect, ref sprite);
        sprite.Scale = new Vector2f(scale, scale);
    }

    public Vector2f Position
    {
        get => sprite.Position;
        set => sprite.Position = value;
    }

    public virtual FloatRect Bounds => sprite.GetGlobalBounds();

    public abstract void Init();

    public abstract void Destroy(Scene scene);
        
    public virtual void Update(float deltaTime) { }

    public virtual void Render(RenderTarget target)
    {
        target.Draw(sprite);
    }
}