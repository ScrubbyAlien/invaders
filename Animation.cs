using SFML.Graphics;

namespace invaders;

public sealed class Animation
{
    public delegate void AnimationEvent(Animation sender);
    public event AnimationEvent? AnimationFinished;
    public event AnimationEvent? FrameFinished;
    
    private readonly float _framelength;
    private readonly float _animationLength; // length 0 means until frames end for non looping or indefinitely for looping
    private int _currentFrame;
    private readonly bool _looping;
    private bool _playingAnimation;
    public readonly string Name;

    public delegate void FrameRenderer(Animatable animatable, RenderTarget target);
    private readonly List<FrameRenderer> _frameRenderers = new();
    private float _frameTimer;
    private float _animationTimer;

    public int CurrentFrame => _currentFrame;
    
    public Animation(string name, bool looping, float fps, float length, FrameRenderer[] frames)
    {
        Name = name;
        _currentFrame = 0;
        _playingAnimation = false;
        _looping = looping;
        _framelength = 1 / (fps == 0 ? 1 : fps);
        _animationLength = length;
        AddFrames(frames);
    }

    public void Play() { _playingAnimation = true; }

    public void Pause() { _playingAnimation = false; }

    public void Unpause() { _playingAnimation = true; }

    public void Reset()
    {
        _currentFrame = 0;
        _frameTimer = 0f;
        _animationTimer = 0f;
        _playingAnimation = false;
    }

    public void DrawFrame(Animatable animatable, RenderTarget target)
    {
        _frameRenderers[_currentFrame](animatable, target);
    }

    public void ProgressAnimation(float deltaTime)
    {
        if (_playingAnimation)
        {
            _animationTimer += deltaTime;
            _frameTimer += deltaTime;
            
            if (_frameTimer >= _framelength)
            {
                _currentFrame++;
                _frameTimer = 0f;
                
                FrameFinished?.Invoke(this);
            }

            if (_currentFrame == _frameRenderers.Count())
            {
                _currentFrame = 0;
                _playingAnimation = _looping;
            }

            if (_animationLength != 0 && _animationTimer >= _animationLength)
            {
                _playingAnimation = false;
                _animationTimer = 0f;
            }
            if (!_playingAnimation) AnimationFinished?.Invoke(this);
        }
    }

    public void AddFrame(FrameRenderer frame)
    {
        _frameRenderers.Add(frame);
    }

    public void AddFrames(FrameRenderer[] frames)
    {
        _frameRenderers.AddRange(frames);
    }
}
