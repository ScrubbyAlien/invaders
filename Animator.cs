using invaders.entity;
using SFML.Graphics;

namespace invaders;

public class Animator
{
    private IntRect _defaultSprite;
    private string _currentAnimation = "";
    private readonly Dictionary<string, Animation> _animationSet = new()
    {
        {"" , new Animation("", false, 0, 0, [])}
    };
    private Entity _instance;

    public bool IsAnimated => _currentAnimation != "";
    private Animation _animation => _animationSet[_currentAnimation];
    
    public Animator(Entity instance)
    {
        _instance = instance;
    }

    public void SetDefaultTextureRect(IntRect rect)
    {
        _defaultSprite = rect;
    }
    
    public void PlayAnimation(string animation, bool fromBeginning)
    {
        if (animation == _currentAnimation && !fromBeginning)
        {
            _animation.Unpause();
        }
        else
        {
            ResetAnimations();
            _currentAnimation = animation;
            _animation.Play();
        }
    }

    public void PauseAnimation() { _animation.Pause(); }

    public void AddAnimation(Animation animation)
    {
        animation.AnimationFinished += AnimationFinished;
        _animationSet.Add(animation.Name, animation);
    }

    public void ProgressAnimation(float deltaTime)
    {
        if (IsAnimated) _animation.ProgressAnimation(deltaTime);
    }

    public void RenderAnimation(RenderTarget target)
    {
        if (IsAnimated) _animation.DrawFrame(_instance, target);
    }

    private void ResetAnimations()
    {
        foreach (KeyValuePair<string,Animation> pair in _animationSet)
        {
            pair.Value.Reset();
        }
    }

    private void AnimationFinished(Animation finished)
    {
        _currentAnimation = "";
        _instance.Sprite.TextureRect = _defaultSprite;
    } 
}