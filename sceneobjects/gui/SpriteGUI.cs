using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects.gui;

public class SpriteGUI : GUI
{
    public SpriteGUI(IntRect initRect) : base("invaders", initRect, Scale) { }

    public void SetScale(float scale)
    {
        sprite.Scale = new Vector2f(scale, scale);
    }
    public void SetScale(Vector2f scale)
    {
        sprite.Scale = scale;
    }

    public void SetColor(Color color)
    {
        sprite.Color = color;
    }
}