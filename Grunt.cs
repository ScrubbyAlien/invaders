using SFML.Graphics;

namespace invaders;

public class Grunt : Actor
{
    public new const int SpriteWidth = 8;
    public new const int SpriteHeight = 8;
    public new const float Scale = 3;
    private static readonly IntRect ANIMATION_STAGE_1 = new(24, 0, SpriteWidth, SpriteHeight);
    private static readonly IntRect ANIMATION_STAGE_2 = new(24, 8, SpriteWidth, SpriteHeight);
    
    public Grunt() : base("invaders", ANIMATION_STAGE_1, Scale) { }

    public override void Init()
    { }

    public override void Destroy(Scene scene)
    { }
}