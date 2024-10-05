namespace invaders;

public static class Animator
{
    private static Dictionary<IAnimatable, float> _animatables = new();

    public static void Animate(float deltaTime)
    {
        foreach (IAnimatable animatable in _animatables.Keys)
        {
            if (_animatables[animatable] >= animatable.AnimationRate)
            {
                animatable.Animate();
                _animatables[animatable] = 0f;
            }
            else
            {
                _animatables[animatable] += deltaTime;
            }
        }
    }

    public static void InitAnimatable(IAnimatable animatable)
    {
        _animatables.Add(animatable, 0f);
    }

    public static void ClearAnimatables()
    {
        _animatables.Clear();
    }
}