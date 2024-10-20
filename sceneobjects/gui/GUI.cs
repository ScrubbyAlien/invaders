using SFML.Graphics;

namespace invaders.sceneobjects.gui;

public abstract class GUI :  RenderObject, ISectionable
{
    private bool _inActiveSelection = true;
    public override bool Active => base.Active && _inActiveSelection;

    public GUI(string textureName, IntRect initRect, float scale) : base(textureName, initRect, scale) { }

    public virtual void SetActiveSelection()
    {
        _inActiveSelection = true;
    }

    public virtual void SetInactiveSelection()
    {
        _inActiveSelection = false;
    }
}