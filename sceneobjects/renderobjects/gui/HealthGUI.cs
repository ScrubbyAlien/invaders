using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects.gui;

public sealed class HealthGUI : SpriteGUI
{
    private int _currentHealth;

    public HealthGUI() : base(TextureRects["healthBar"])
    {
        // needs to subscribe before player publishes its full health in it's initialize step
        GlobalEventManager.PlayerChangeHealth += OnHealthChange;
    }

    public override void Destroy()
    {
        GlobalEventManager.PlayerChangeHealth -= OnHealthChange;
    }

    public override void Render(RenderTarget target)
    {
        for (int i = 0; i < _currentHealth; i++)
        {
            if (i == _currentHealth - 1)
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

        }
    }

    private void OnHealthChange(int diff)
    {
        // if (diff != 0) Console.WriteLine(diff);
        _currentHealth += diff;
    }
}