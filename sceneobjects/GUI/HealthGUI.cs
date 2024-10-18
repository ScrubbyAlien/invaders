using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public sealed class HealthGUI : SpriteGUI
{
    private int currentHealth;

    public HealthGUI() : base(TextureRects["healthBar"]) { }

    protected override void Initialize()
    {
        if (Scene.FindByType(out Player? p))
        {
            currentHealth = p!.CurrentHealth;
        }
        EventManager.PlayerChangeHealth += OnHealthChange;
    }

    public override void Destroy()
    {
        EventManager.PlayerChangeHealth -= OnHealthChange;
    }

    public override void Render(RenderTarget target)
    {
        for (int i = 0; i < currentHealth; i++)
        {
            if (i == currentHealth - 1)
            {
                sprite.TextureRect = TextureRects["healthBarEnd"];
                Position = new Vector2f(
                    (TextureRects["guiBackgroundLeft"].Width - 3) * Scale - TextureRects["healthBar"].Width * Scale * i - Scale * 2,
                    35
                );
                target.Draw(sprite);
                continue;
            }
            sprite.TextureRect = TextureRects["healthBar"];
            Position = new Vector2f(
                (TextureRects["guiBackgroundLeft"].Width - 3) * Scale - Bounds.Width * i,
                35
            );
            target.Draw(sprite);
             
                
                // MiddleOfScreen(
                // Bounds, 
                // new Vector2f(
                // -TextureRects["guiBackgroundMiddle"].Width * Scale / 2f - 2 - Bounds.Width * i, 
                // -Program.ScreenHeight / 2f + 40
                // )); 
            
        }
    }

    private void OnHealthChange(int diff)
    {
        currentHealth += diff;
    }
}