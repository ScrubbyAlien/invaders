using SFML.Graphics;

namespace invaders.sceneobjects;

public abstract class GUI :  RenderObject
{
    public GUI(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }
    
    
}