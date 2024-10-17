using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects;

public class TextGUI : GUI
{
    protected Text text = new();
    private Alignment _alignment;
    
    public override Vector2f Position
    {
        get => text.Position;
        set => text.Position = value;
    }

    public override FloatRect Bounds => text.GetGlobalBounds();

    public TextGUI(string displayText, Alignment alignment = Alignment.Center) : base("invaders", new IntRect(), 1)
    {
        _alignment = alignment;
        text.DisplayedString = displayText;
        text.Font = AssetManager.LoadFont("pixel-font");
        text.CharacterSize = 10 * (int)Scale;
        text.FillColor = Color.White;
        zIndex = 200;
        AlignText();
    }

    protected override void Initialize()
    {
        Animation blink = new Animation("blink", true, 3, 0, blinking);
        animator.AddAnimation(blink);
        base.Initialize();
    }
    
    public override void Render(RenderTarget target)
    {
        if (animator.IsAnimated) animator.RenderAnimation(target);
        else target.Draw(text);
    }

    public void SetText(string newText)
    {
        text.DisplayedString = newText;
        AlignText();
    }

    public void SetFillColor(Color color)
    {
        text.FillColor = color;
    }

    public override Animatable GetAnimatable()
    {
        return new Animatable(this, text, animator);
    }

    private void AlignText()
    {
        if (text.DisplayedString == "") return;
        if (_alignment == Alignment.Left) return;
        
        // create dictionary
        Dictionary<char, float> characterToWidth = new()
        {
            { ' ', 8f } // don't know why 8 is the magic number, it just works
        };
        HashSet<char> charactersInText = new() { ' ' };
        foreach (char c in text.DisplayedString)
        {
            if (charactersInText.Contains(c)) continue;
            charactersInText.Add(c);
        }
        foreach (char c in charactersInText)
        {
            if (characterToWidth.ContainsKey(c)) continue;
            characterToWidth.Add(c, text.Font.GetGlyph(c, text.CharacterSize, false, 0).Bounds.Width);
        }
        // calculate line lengths
        List<string> lines = text.DisplayedString.Split("\n").ToList();
        if (lines.Count == 1) return;
        List<float> lineLengths =
            lines.Select(line =>
            {
                return line.ToCharArray()
                    .ToList()
                    .Select(c => characterToWidth[c]) // convert characters to their width
                    .Aggregate((f1, f2) => f1 + f2); // summate all widths to lines total width
            }).ToList();
        float longestLine = lineLengths.Max();
        
        // calculate and apply padding
        for (int i = 0; i < lines.Count; i++)
        {
            float length = lineLengths[i];
            float padding = longestLine - length;
            if (padding > 0)
            {
                float spaceWidth = characterToWidth[' '];
                int paddingSpaces = (int) MathF.Ceiling(padding / spaceWidth); // round up to integer
                switch (_alignment)
                {
                    case Alignment.Center:
                        for (int j = 0; j < paddingSpaces / 2; j++)
                        {
                            lines[i] = " " + lines[i] + " ";
                        }
                        break;
                    case Alignment.Right:
                        for (int j = 0; j < paddingSpaces; j++)
                        {
                            lines[i] = " " + lines[i];
                        }
                        break;
                }
            }
        }
        text.DisplayedString = String.Join("\n", lines);
    }

    private Animation.FrameRenderer[] blinking =
    [
        (_, _) => { },
        (animatable, target) =>
        {
            target.Draw(animatable.Text);
        }
    ];

    public enum Alignment
    {
        Left, Center, Right
    }
}