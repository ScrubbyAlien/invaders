using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects.renderobjects.gui;

public sealed class FadingTextGUI : TextGUI
{
    private readonly float _fadeTime;
    private float _fadeTimer;
    private readonly Vector2f _drift;

    public FadingTextGUI(float fadeTime, string displayText, uint size = 20, Vector2f drift = new()) : base(displayText,
        size) {
        zIndex = 500;
        _fadeTime = fadeTime;
        text.CharacterSize = size;
        _drift = drift;
    }

    public override void Update(float deltaTime) {
        base.Update(deltaTime);

        Position += _drift * deltaTime;

        _fadeTimer += deltaTime;
        float percent = _fadeTimer / _fadeTime;
        byte alpha = (byte)MathF.Round(255 - 255 * percent);
        Color fading = new Color(255, 255, 255, alpha);
        text.FillColor = fading;
        if (_fadeTimer >= _fadeTime) {
            Scene.QueueDestroy(this);
        }
    }
}