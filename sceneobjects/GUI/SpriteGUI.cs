using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects;

public class SpriteGUI : GUI
{
    public SpriteGUI(IntRect initRect) : base("invaders", initRect, Scale) { }

    public void SetScale(float scale)
    {
        sprite.Scale = new Vector2f(scale, scale);
    }
}