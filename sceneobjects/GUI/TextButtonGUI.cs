using invaders.interfaces;
using SFML.Graphics;

namespace invaders.sceneobjects;

public class TextButtonGUI : TextGUI, IClickable
{
    public event IClickable.ClickedEvent? Clicked;

    public TextButtonGUI(string buttonText) : base(buttonText) { }

    protected override void Initialize()
    {
        Animation selected = new Animation("selected", true, 60, 0, selectedFrames);
        animator.AddAnimation(selected);
    }

    public void Select()
    {
        animator.PlayAnimation("selected", true);
    }

    public void Unselect()
    {
        animator.StopAnimation();
        text.FillColor = Color.White;
    }
    
    public void Click()
    {
        Clicked?.Invoke(this);
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