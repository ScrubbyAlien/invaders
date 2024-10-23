using SFML.Graphics;

namespace invaders.sceneobjects.renderobjects.gui;

public sealed class TextButtonGUI : TextGUI, IClickable
{
    private const float _darkGray = 50;
    private const float _white = 255;

    public event Action? Clicked;

    public TextButtonGUI(string buttonDisplayText) : base(buttonDisplayText) {
        Deactivate();
    }

    protected override void Initialize() {
        Animation selected = new Animation("selected", true, 60, 0, selectedFrames);
        animator.AddAnimation(selected);
    }

    public void Select() {
        animator.PlayAnimation("selected", true);
    }

    public void Deselect() {
        animator.StopAnimation();
        SetFillColor(Color.White);
    }

    public override void SetInactiveSelection() {
        base.SetInactiveSelection();
        Deselect();
    }


    public void Click() {
        Clicked?.Invoke();
    }

    private readonly Animation.FrameRenderer[] selectedFrames = [
        (animatable, target) =>
        {
            float progress = animatable.Animator.FrameCount / 13f;
            progress %= MathF.PI; // resulting sin value should always be positive
            byte lerp = (byte)MathF.Round(float.Lerp(_white, _darkGray, MathF.Sin(progress)));
            Color lerpedColor = new Color(lerp, lerp, lerp);
            animatable.Text.FillColor = lerpedColor;
            target.Draw(animatable.Text);
        }
    ];
}