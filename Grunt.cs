using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace invaders;

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
    
    public Grunt(int rank) : base(rank, "invaders", AnimationStages[0], Scale) { }

    public override void Init()
    {
        _horizontalSpeed = 30f;
        RankHitWall += OnRankHitWall;
    }
    
    public override void Destroy()
    {
        RankHitWall -= OnRankHitWall;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
    }


    protected override void OnOutsideScreen((ScreenState x, ScreenState y) state, Vector2f outsidePos, out Vector2f adjustedPos)
    {
        base.OnOutsideScreen(state, outsidePos, out adjustedPos);
        
        switch (state.x)
        {
            case ScreenState.OutSideLeft: 
                adjustedPos.X = Scene.MarginSide;
                InvokeRankHitWall();
                break;
            case ScreenState.OutSideRight: 
                adjustedPos.X = Program.ScreenWidth - Bounds.Width - Scene.MarginSide;
                InvokeRankHitWall();
                break;
        }
    }

    private void OnRankHitWall(AbstractEnemy sender, int rank)
    {
        if (rank == Rank) Reverse();
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