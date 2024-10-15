using invaders.sceneobjects;
using SFML.Graphics;

namespace invaders;

public class Animator
{
    private IntRect _defaultSprite;
    private int _frameCount;
    private string _currentAnimation = "";
    private readonly Dictionary<string, Animation> _animationSet = new()
    {
        {"" , new Animation("", false, 0, 0, [])}
    };
    private RenderObject _instance;

    public bool IsAnimated => _currentAnimation != "";
    public Animation CurrentAnimation => _animationSet[_currentAnimation];
    public int FrameCount => _frameCount;

    public Animator(RenderObject instance)
    {
        _instance = instance;
    }
    
    public void SetDefaultSprite(IntRect rect)
    {
        _defaultSprite = rect;
    }
    
    public void PlayAnimation(string animation, bool fromBeginning)
    {
        if (animation == _currentAnimation && !fromBeginning)
        {
            CurrentAnimation.Unpause();
        }
        else
        {
            ResetAnimations();
            _frameCount = 0;
            _currentAnimation = animation;
            CurrentAnimation.Play();
        }
    }

    public void PauseAnimation() { CurrentAnimation.Pause(); }

    public void AddAnimation(Animation animation)
    {
        animation.AnimationFinished += AnimationFinished;
        animation.FrameFinished += s => _frameCount++;
        _animationSet.Add(animation.Name, animation);
    }

    public void ProgressAnimation(float deltaTime)
    {
        if (IsAnimated) CurrentAnimation.ProgressAnimation(deltaTime);
    }

    public void RenderAnimation(RenderTarget target)
    {
        if (IsAnimated) CurrentAnimation.DrawFrame(_instance.GetAnimatable(), target);
    }

    private void ResetAnimations()
    {
        _frameCount = 0;
        foreach (KeyValuePair<string,Animation> pair in _animationSet)
        {
            pair.Value.Reset();
        }
    }

    private void AnimationFinished(Animation finished)
    {
        _frameCount = 0;
        _currentAnimation = "";
        _instance.GetAnimatable().SetTextureRect(_defaultSprite);
    } 
}