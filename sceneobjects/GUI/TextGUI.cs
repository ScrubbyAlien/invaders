using SFML.Graphics;
using SFML.System;

namespace invaders.sceneobjects;

public class TextGUI : GUI
{
    protected Text text = new();
    
    public override Vector2f Position
    {
        get => text.Position;
        set => text.Position = value;
    }

    public override FloatRect Bounds => text.GetGlobalBounds();

    public TextGUI(string text, bool centerAligned = true) : base("invaders", new IntRect(), 1)
    {
        this.text.DisplayedString = text;
        this.text.Font = AssetManager.LoadFont("pixel-font");
        this.text.CharacterSize = 10 * (int)Scale;
        this.text.FillColor = Color.White;
        zIndex = 200;
        if (centerAligned) CenterText();
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
    }

    public void SetFillColor(Color color)
    {
        text.FillColor = color;
    }

    public override Animatable GetAnimatable()
    {
        return new Animatable(this, text, animator);
    }

    private void CenterText()
    {
        if (text.DisplayedString == "") return;
        
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
                for (int j = 0; j < paddingSpaces / 2; j++)
                {
                    lines[i] = " " + lines[i] + " ";
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
}