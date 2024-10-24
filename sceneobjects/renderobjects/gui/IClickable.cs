namespace invaders.sceneobjects.renderobjects.gui;

public interface IClickable : INavigatable
{
    public event Action? Clicked;

    public void Click();
    public void Select();
    public void Deselect();
}