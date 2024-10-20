using SFML.Graphics;

namespace invaders.sceneobjects.gui;

public sealed class TextButtonGUI : TextGUI, IClickable
{
    public event Action? Clicked;

    public TextButtonGUI(string buttonDisplayText) : base(buttonDisplayText)
    {
        Deactivate();
    }

    protected override void Initialize()
    {
        Animation selected = new Animation("selected", true, 60, 0, selectedFrames);
        animator.AddAnimation(selected);
    }

    public void Select()
    {
        animator.PlayAnimation("selected", true);
    }
    public void Deselect()
    {
        animator.StopAnimation();
        SetFillColor(Color.White);
    }
    
    public override void SetInactiveSelection()
    {
        base.SetInactiveSelection();
        Deselect();
    }


    public void Click() { Clicked?.Invoke(); }

    private Animation.FrameRenderer[] selectedFrames =
    [
        (animatable, target) =>
        {
            float darkGray = 50;
            float white = 255;
            float progress = animatable.Animator.FrameCount / 13f;
            progress %= MathF.PI; // keep value within PI radians
            byte lerp = (byte) MathF.Round(float.Lerp(white, darkGray, MathF.Sin(progress)));
            Color lerpedColor = new Color(lerp, lerp, lerp);
            animatable.Text.FillColor = lerpedColor;
            target.Draw(animatable.Text);
        },
    ];
}