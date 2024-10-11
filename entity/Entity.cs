using invaders.enums;
using SFML.Graphics;
using SFML.System;

namespace invaders.entity;

public abstract class Entity
{
    protected Sprite sprite = new();
    protected string textureName;
    protected const float Scale = 3;
    protected Animator animator;
    public bool Dead = false;
    public bool DontDestroyOnClear = false;
    public int zIndex;
    private bool _initialized;

    public Entity(string textureName, IntRect initRect, float scale)
    {
        animator = new(this);
        this.textureName = textureName;
        AssetManager.LoadTexture(textureName, initRect, ref sprite);
        sprite.Scale = new Vector2f(scale, scale); // scale is constructor parameter so it can be changed by children
    }

    public bool Initialized => _initialized;
    
    public Vector2f Position
    {
        get => sprite.Position;
        set => sprite.Position = value;
    }

    public virtual FloatRect Bounds => sprite.GetGlobalBounds();

    public virtual CollisionLayer Layer => CollisionLayer.None;
    
    /// <summary>
    /// Any functionality that requires references to other entities should be called from Initialize,
    /// such as FindByType calls, or event handlers/listerners
    /// </summary>
    protected virtual void Initialize() { }

    public void FullInitialize()
    {
        Initialize();
        _initialized = true;
    }

    public virtual void Destroy() {}

    public virtual void Update(float deltaTime)
    {
        animator.ProgressAnimation(deltaTime);
    }

    public virtual void Render(RenderTarget target)
    {
        if (animator.IsAnimated) animator.RenderAnimation(target);
        else target.Draw(sprite);
    }
    

    public static int CompareByZIndex(Entity? e1, Entity? e2)
    {
        if (e1 == null && e2 == null) return 0;
        if (e1 == null) return -1;
        if (e2 == null) return 1;
        return e1.zIndex - e2.zIndex;
    }

    protected static Animation.FrameRenderer BasicFrameRenderer(IntRect rect)
    {
        return (animatable, target) =>
        {
            animatable.SetTextureRect(rect);
            target.Draw(animatable.Sprite);
        };
    }

    public Animatable GetAnimatable()
    {
        return new Animatable(this, sprite, animator);
    }
}

public class Animatable(Entity i, Sprite s, Animator a)
{
    public Entity Instance = i;
    public Sprite Sprite = s;
    public Animator Animator = a;
    
    public void SetTextureRect(IntRect rect)
    {
        Sprite.TextureRect = rect;
    }
}
