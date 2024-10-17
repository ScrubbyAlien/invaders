using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects;

public class HealthGUI : GUI
{
    private int currentHealth;

    public HealthGUI() : base("invaders", TextureRects["healthBar"], Scale)
    {
        zIndex = 100;
    }

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
            Position = new Vector2f(12, 878 - i * TextureRects["healthBar"].Height * Scale);
            target.Draw(sprite);
        }
    }

    private void OnHealthChange(int diff)
    {
        currentHealth += diff;
    }
}