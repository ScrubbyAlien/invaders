using SFML.Graphics;
using SFML.System;
using invaders.interfaces;

namespace invaders.entity;

public class Grunt : AbstractEnemy, IAnimatable
{
    public new const int SpriteWidth = 8;
    public new const int SpriteHeight = 8;
    public new const float Scale = 3;

    public static IntRect[] AnimationStages => new IntRect[2]
    {
        new(24, 0, SpriteWidth, SpriteHeight),
        new(24, 8, SpriteWidth, SpriteHeight)
    };
    public float AnimationRate => 0.3f;
    private IAnimatable.AnimationStage _animStage = IAnimatable.AnimationStage.Stage1;
    
    public Grunt(int wave) : base(wave, "invaders", AnimationStages[0], Scale) { }

    public override void Init()
    {
        _horizontalSpeed = new Random().Next(2) == 0 ? 30f : -30f;
    }
    
    public override void Destroy() { }

    protected override void OnOutsideScreen((ScreenState x, ScreenState y) state, Vector2f outsidePos, out Vector2f adjustedPos)
    {
        base.OnOutsideScreen(state, outsidePos, out adjustedPos);
        
        switch (state.x)
        {
            case ScreenState.OutSideLeft: 
                adjustedPos.X = Scene.MarginSide;
                Reverse();
                break;
            case ScreenState.OutSideRight: 
                adjustedPos.X = Program.ScreenWidth - Bounds.Width - Scene.MarginSide;
                Reverse();
                break;
        }
    }
    
    public void Animate()
    {
        switch (_animStage)
        {
            case IAnimatable.AnimationStage.Stage1:
                sprite.TextureRect = AnimationStages[1];
                _animStage = IAnimatable.AnimationStage.Stage2;
                break;
            case IAnimatable.AnimationStage.Stage2:
                sprite.TextureRect = AnimationStages[0];
                _animStage = IAnimatable.AnimationStage.Stage1;
                break;
        }
    }
}