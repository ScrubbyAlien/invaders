using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects.renderobjects.gui;

public class SpriteGUI : GUI
{
    private const float _transparant = 50;
    private const float _opaque = 255;

    private IntRect _availableArea;

    public IntRect AvailableArea {
        get {
            IntRect scaled = new(
                _availableArea.Left * (int)Scale,
                _availableArea.Top * (int)Scale,
                _availableArea.Width * (int)Scale,
                _availableArea.Height * (int)Scale
            );
            return scaled;
        }
    }

    public SpriteGUI(IntRect initRect) : base("invaders", initRect, Scale) {
        _availableArea = new IntRect(0, 0, initRect.Width, initRect.Height);
        Animation pulsing = new("pulsing", true, 60, 0, pulsingFrames);
        Animation opaque = new("opaque", false, 1, 0, opaqueFrames);
        animator.AddAnimation(pulsing);
        animator.AddAnimation(opaque);
    }

    public void SetAvailableArea(IntRect area) => _availableArea = area;

    public Vector2f GetPositionInAvailableArea(Vector2f position = new()) {
        Vector2f positionInArea = Position +
                                  new Vector2f(AvailableArea.Left, AvailableArea.Top) +
                                  position;
        return positionInArea;
    }

    public void SetScale(float scale) => sprite.Scale = new Vector2f(scale, scale);

    public void SetScale(Vector2f scale) => sprite.Scale = scale;

    public void SetColor(Color color) => sprite.Color = color;

    private readonly Animation.FrameRenderer[] pulsingFrames = [
        (animatable, target) => {
            float progress = animatable.Animator.FrameCount / 5f;
            progress %= MathF.PI; // resulting sin value should always be positive
            byte lerp = (byte)MathF.Round(float.Lerp(_opaque, _transparant, MathF.Sin(progress)));
            Color c = animatable.Sprite.Color;
            Color lerped = new(c.R, c.G, c.B, lerp);
            animatable.Sprite.Color = lerped;
            target.Draw(animatable.Sprite);
        },
    ];

    private readonly Animation.FrameRenderer[] opaqueFrames = [
        (animatable, target) => {
            Color c = animatable.Sprite.Color;
            Color opaque = new(c.R, c.G, c.B, 255);
            animatable.Sprite.Color = opaque;
            target.Draw(animatable.Sprite);
        },
    ];
}