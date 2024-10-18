using invaders.interfaces;
using SFML.Graphics;

namespace invaders.sceneobjects;

public sealed class TextButtonGUI : TextGUI, IClickable
{
    public event Action? Clicked;
    private bool _unavailable;
    
    public TextButtonGUI(string buttonDisplayText) : base(buttonDisplayText)
    {
        Deactivate();
    }

    protected override void Initialize()
    {
        Animation selected = new Animation("selected", true, 60, 0, selectedFrames);
        animator.AddAnimation(selected);
    }

    public void Select() { animator.PlayAnimation("selected", true); }
    public void Unselect()
    {
        animator.StopAnimation();
        text.FillColor = Color.White;
    }
    public void Activate() { _unavailable = false; }
    public void Deactivate() { _unavailable = true; }
    public void Click() { Clicked?.Invoke(); }

    public override void Render(RenderTarget target)
    {
        if (_unavailable) text.FillColor = new Color(80, 80, 80, 80);
        else text.FillColor = Color.White;
        base.Render(target);
    }

    private Animation.FrameRenderer[] selectedFrames =
    [
        (animatable, target) =>
        {
            float darkGray = 50;
            float white = 255;
            float progress = animatable.Animator.FrameCount / 13f;
            progress %= MathF.PI; // keep value within PI radians
            byte lerp = (byte) (int) MathF.Round(float.Lerp(white, darkGray, MathF.Sin(progress)));
            Color lerpedColor = new Color(lerp, lerp, lerp);
            animatable.Text.FillColor = lerpedColor;
            target.Draw(animatable.Text);
        },
    ];
}