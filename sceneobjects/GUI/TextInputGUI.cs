using SFML.Graphics;
using static SFML.Window.Keyboard.Key;
using static invaders.Utility;
using SFML.System;
using SFML.Window;

namespace invaders.sceneobjects;

public sealed class TextInputGUI : TextGUI
{
    public event EventHandler<string>? InputEntered;
    
    private TextGUI _caret;
    private int _maxCharacterCount;
    private string _validCharacters = "abcdefghijklmnopqrstuvwxyzåäö1234567890:;()/<>- ";
    private Func<RenderObject, Vector2f> _positionCalculator = o => o.Position;
    private bool _backspacePressed;
    private bool _enterPressed;


    public override bool Paused
    {
        get => paused;
        set 
        {
            paused = value;
            _caret.Paused = value;
            if (value) _caret.Hide();
            else _caret.Unhide();
        }
    }

    public TextInputGUI(int maxCharacterCount, uint size = 10, Alignment alignment = Alignment.Center) : base("", size, alignment)
    {
        _maxCharacterCount = maxCharacterCount;
        _caret = new TextGUI("|", size + 2, alignment);
        Scene.TextEntered += OnTextEntered;
        Scene.QueueSpawn(_caret);
        Scene.DeferredCall(_caret.GetAnimatable().Animator, "PlayAnimation", ["blink", true]);
    }

    public override void Update(float deltaTime)
    {
        if (AreAnyKeysPressed([Backspace]))
        {
            if (!_backspacePressed)
            {
                _backspacePressed = true;
                RemoveLastCharacter();
            }
        }
        else _backspacePressed = false;

        if (AreAnyKeysPressed([Enter]) && text.DisplayedString.Length > 0)
        {
            if (!_enterPressed)
            {
                _enterPressed = true;
                InputEntered?.Invoke(this, text.DisplayedString);
            }
        }
        else _enterPressed = false;
        
        Position = _positionCalculator(this);
        Vector2f _caretPosition = new Vector2f(Position.X + Bounds.Width + 4, Position.Y - 5);
        _caret.Position = _caretPosition;
        if (text.DisplayedString.Length == 0)
        {
            FloatRect glyphBounds = text.Font.GetGlyph('|', text.CharacterSize, false, 0).Bounds;
            _caret.Position = _caretPosition + new Vector2f(-glyphBounds.Width / 2f, -glyphBounds.Height / 2f);
        }
    }

    public void PositionCalculator(Func<RenderObject, Vector2f> positionCalculator)
    {
        _positionCalculator = positionCalculator;
    }

    private void OnTextEntered(object? o, TextEventArgs args)
    {
        if (text.DisplayedString.Length < _maxCharacterCount)
        {
            char c = Convert.ToChar(args.Unicode);
            if (_validCharacters.Contains(c.ToString().ToLower()))
            {
                text.DisplayedString += c;
            }
        }
    }

    private void RemoveLastCharacter()
    {
        string display = text.DisplayedString;
        if (display.Length > 0)
        {
            text.DisplayedString = display.Substring(0, display.Length - 1);
        }
    }
}