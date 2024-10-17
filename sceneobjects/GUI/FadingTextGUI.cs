using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects;

public class FadingTextGUI : TextGUI
{
    private float _fadeTime;
    private float _fadeTimer;
    private Vector2f _drift;
    
    public FadingTextGUI(float fadeTime, string displayText, uint size = 20) : base(displayText, Alignment.Center)
    {
        _fadeTime = fadeTime;
        text.CharacterSize = size;
    }
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        Position += _drift * deltaTime;
        
        _fadeTimer += deltaTime;
        float percent = _fadeTimer / _fadeTime;
        byte alpha = (byte) MathF.Round(255 - 255 * percent);
        Color fading = new Color(255, 255, 255, alpha);
        text.FillColor = fading;
        if (_fadeTimer >= _fadeTime)
        {
            Scene.QueueDestroy(this);
        }
    }

    public void SetDrift(Vector2f drift) => _drift = drift;
}