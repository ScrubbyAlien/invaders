using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Keyboard.Key;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects.gui;

public sealed class TextInputGUI : TextGUI
{
    public event EventHandler<string>? InputEntered;

    private readonly TextGUI _caret;
    private readonly int _maxCharacterCount;
    private const string _validCharacters = "abcdefghijklmnopqrstuvwxyzåäö1234567890:()/<>- ";
    private Func<RenderObject, Vector2f> _positionCalculator = o => o.Position;
    private bool _backspacePressed;
    private bool _enterPressed;

    public TextInputGUI(int maxCharacterCount, uint size = 10, Alignment alignment = Alignment.Center) : base("", size,
        alignment) {
        _maxCharacterCount = maxCharacterCount;
        _caret = new TextGUI("|", size + 2, alignment);
        Scene.TextEntered += OnTextEntered;
        _caret.Spawn();
        Scene.DeferredCall(_caret.GetAnimatable().Animator, "PlayAnimation", ["blink", true]);
    }

    public override void Update(float deltaTime) {
        if (AreAnyKeysPressed([Backspace])) {
            if (!_backspacePressed) {
                _backspacePressed = true;
                RemoveLastCharacter();
            }
        }
        else {
            _backspacePressed = false;
        }

        if (AreAnyKeysPressed([Enter]) && text.DisplayedString.Length > 0) {
            if (!_enterPressed) {
                _enterPressed = true;
                InputEntered?.Invoke(this, text.DisplayedString);
            }
        }
        else {
            _enterPressed = false;
        }

        Position = _positionCalculator(this);
        Vector2f caretPosition = new(Position.X + Bounds.Width + 4, Position.Y - 5);
        _caret.Position = caretPosition;
        if (text.DisplayedString.Length == 0) {
            FloatRect glyphBounds = text.Font.GetGlyph('|', text.CharacterSize, false, 0).Bounds;
            _caret.Position = caretPosition + new Vector2f(-glyphBounds.Width * Scale / 2f, -glyphBounds.Height / 2f);
        }
    }

    public void PositionCalculator(Func<RenderObject, Vector2f> positionCalculator) =>
        _positionCalculator = positionCalculator;

    private void OnTextEntered(object? o, TextEventArgs args) {
        if (text.DisplayedString.Length < _maxCharacterCount) {
            char c = Convert.ToChar(args.Unicode);
            if (_validCharacters.Contains(c.ToString().ToLower())) {
                text.DisplayedString += c;
            }
        }
    }

    private void RemoveLastCharacter() {
        string display = text.DisplayedString;
        if (display.Length > 0) {
            // extract the range from index zero up until and including the second to last index
            // effectively removing the last character from the range
            // https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/ranges-indexes
            text.DisplayedString = display[0..^1];
        }
    }

    public override void Pause() {
        base.Pause();
        _caret.Pause();
        _caret.Hide();
    }

    public override void Unpause() {
        base.Unpause();
        _caret.Unpause();
        _caret.Unhide();
    }
}