namespace invaders;

public static class EventManager
{
    public static void BroadcastEvents()
    {
        BroadcastPlayerChangeHealth();
        BroadcastBackgroundSetScrollSpeed();
    }
    
    public delegate void ValueChangeEvent<T>(T diff); // FIX: T should be value type
    
    public static event ValueChangeEvent<int>? PlayerChangeHealth;
    private static int _playerHealthDiff;
    public static void PublishPlayerChangeHealth(int diff) { _playerHealthDiff += diff; }
    public static void BroadcastPlayerChangeHealth()
    {
        PlayerChangeHealth?.Invoke(_playerHealthDiff);
        _playerHealthDiff = 0;
    }


    public delegate void ValueSetLerpEvent<T>(T newValue, float lerpTime);
    
    public static event ValueSetLerpEvent<float>? BackgroundSetScrollSpeed;
    private static float _newBackgroundScroll = Settings.AmbientScrollInLevel;
    private static float _newBackgroundScrollLerpTime;
    public static void PublishBackgroundSetScrollSpeed(float newScroll, float lerpTime)
    {
        _newBackgroundScroll = newScroll;
        _newBackgroundScrollLerpTime = lerpTime;
    }
    public static void BroadcastBackgroundSetScrollSpeed() { 
        BackgroundSetScrollSpeed?.Invoke(_newBackgroundScroll, _newBackgroundScrollLerpTime);
    }
}