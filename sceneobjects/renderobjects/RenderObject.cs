using invaders.enums;
using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects.renderobjects;

public abstract class RenderObject : SceneObject
{
    protected readonly Sprite sprite = new();
    public const float Scale = 4;
    protected readonly Animator animator;
    protected int zIndex;
    private bool _hidden;
    public bool Hidden => _hidden;

    protected RenderObject(string textureName, IntRect initRect, float scale) {
        animator = new Animator(this);
        if (initRect.Width != 0) sprite.Texture = AssetManager.LoadTexture(textureName);
        sprite.TextureRect = initRect;
        sprite.Scale = new Vector2f(scale, scale); // scale is constructor parameter so it can be changed by children
    }

    public virtual Vector2f Position {
        get => sprite.Position;
        set => sprite.Position = value;
    }

    public virtual FloatRect Bounds => sprite.GetGlobalBounds();

    public virtual CollisionLayer Layer { get; set; } = CollisionLayer.None;

    public override void Update(float deltaTime) => animator.ProgressAnimation(deltaTime);

    public virtual void Render(RenderTarget target) {
        if (animator.IsAnimated) {
            animator.RenderAnimation(target);
        }
        else {
            target.Draw(sprite);
        }
    }

    public void Hide() => _hidden = true;
    public void Unhide() => _hidden = false;

    public void SetZIndex(int index) => zIndex = index;

    public static int CompareByZIndex(RenderObject? e1, RenderObject? e2) {
        if (e1 == null && e2 == null) return 0;
        if (e1 == null) return -1;
        if (e2 == null) return 1;
        return e1.zIndex - e2.zIndex;
    }

    protected static Animation.FrameRenderer BasicFrameRenderer(IntRect rect) =>
        (animatable, target) => {
            animatable.SetTextureRect(rect);
            target.Draw(animatable.Sprite);
        };

    public virtual Animatable GetAnimatable() => new(this, sprite, animator);
}