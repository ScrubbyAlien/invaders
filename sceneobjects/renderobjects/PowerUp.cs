using invaders.enums;
using static invaders.Utility;
using SFML.System;

namespace invaders.sceneobjects.renderobjects;

public sealed class PowerUp : RenderObject
{
    private readonly Types _type;
    private Background? _background;

    public static Dictionary<string, Types> StringToType = new()
    {
        {"triple", Types.TripleShot},
        {"health", Types.RepairShip},
        {"speed", Types.ThrusterBoost}
    };

    public PowerUp(string name, Vector2f position) : base("invaders", TextureRects[name + "Power"] , Scale)
    {
        Layer = CollisionLayer.PowerUp;
        Position = position;
        _type = StringToType[name];
    }

    protected override void Initialize()
    {
        _background = Scene.FindByType<Background>();
        // if there is no invasion going on there shouldn't be any power ups on screen
        if (Scene.FindAllByType<Invasion>().Count() == 0) Scene.QueueDestroy(this);
    }

    public override void Update(float deltaTime)
    {
        Vector2f velocity = new Vector2f(0, _background?.ScrollSpeed ?? 0);
        Position += velocity * deltaTime;
        
        if (Position.Y > Program.ScreenHeight) Scene.QueueDestroy(this);
    }

    public Types Absorb()
    {
        Scene.QueueDestroy(this);
        return _type;
    }

    public enum Types
    {
        TripleShot,
        RepairShip,
        ThrusterBoost
    }
}