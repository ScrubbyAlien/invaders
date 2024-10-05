using SFML.Graphics;

namespace invaders;

public interface IAnimatable
{
    public static IntRect[] AnimationStages { get; } = null!;
    public float AnimationRate { get; }
    public void Animate();
    
    public enum AnimationStage
    {
        Stage1,
        Stage2,
        Stage3,
        Stage4
    }
}