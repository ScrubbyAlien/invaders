using SFML.Graphics;
using SFML.System;

namespace invaders.entity.GUI;

public abstract class GUI :  Entity
{
    public const int GuiMargin = 50;

    public GUI(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }
}