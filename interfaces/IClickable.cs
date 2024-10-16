namespace invaders.interfaces;

public interface IClickable
{
    public event Action? Clicked;
    
    public void Click();
    public void Select();
    public void Unselect();
}