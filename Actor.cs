using SFML.Graphics;

namespace invaders;

public abstract class Actor : Entity
{
    public Actor(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }

    // implement movement functions, children determine specifc movement behaviour
    
}