using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects;

public class HealthGUI : GUI
{
    private static readonly IntRect Bar = new(0, 40, 8, 2);
    private int currentHealth;

    public HealthGUI() : base("invaders", Bar, Scale)
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
            Position = new Vector2f(12, 878 - i * Bar.Height * Scale);
            target.Draw(sprite);
        }
    }

    private void OnHealthChange(int diff)
    {
        currentHealth += diff;
    }
}