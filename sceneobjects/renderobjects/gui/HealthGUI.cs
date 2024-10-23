using invaders.enums;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects.gui;

public sealed class HealthGUI : SpriteGUI
{
    private int _currentHealth;
    private SpriteGUI _leftGui = null!;

    public HealthGUI() : base(TextureRects["healthBar"]) {
        // needs to subscribe before player publishes its full health in it's initialize step
        GlobalEventManager.PlayerChangeHealth += OnHealthChange;
    }

    protected override void Initialize() => _leftGui = Scene.FindByTag<SpriteGUI>(SceneObjectTag.GuiBackgroundLeft)!;

    public override void Destroy() => GlobalEventManager.PlayerChangeHealth -= OnHealthChange;

    public override void Render(RenderTarget target) {
        for (int i = 0; i < _currentHealth; i++) {
            if (i == _currentHealth - 1) {
                sprite.TextureRect = TextureRects["healthBarEnd"];
                Position = _leftGui.GetPositionInAvailableArea(new Vector2f(
                    _leftGui.AvailableArea.Width - TextureRects["healthBar"].Width * Scale * i - Scale * 2,
                    (_leftGui.AvailableArea.Height - Bounds.Height) / 2f
                ));
                target.Draw(sprite);
                continue;
            }

            sprite.TextureRect = TextureRects["healthBar"];
            Position = _leftGui.GetPositionInAvailableArea(new Vector2f(
                _leftGui.AvailableArea.Width - TextureRects["healthBar"].Width * Scale * i,
                (_leftGui.AvailableArea.Height - Bounds.Height) / 2f
            ));
            target.Draw(sprite);
        }
    }

    private void OnHealthChange(int diff) => _currentHealth += diff;
}