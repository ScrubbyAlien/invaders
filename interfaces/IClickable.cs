namespace invaders.interfaces;

public interface IClickable
{
    public delegate void ClickedEvent(IClickable button);
    public event ClickedEvent? Clicked;
    
    public void Click();
    public void Select();
    public void Unselect();
}