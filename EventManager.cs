namespace invaders;

public static class EventManager
{
    public static void BroadcastEvents()
    {
        BroadcastPlayerChangeHealth();
        BroadcastBackgroundSetScrollSpeed();
        BroadcastPlayerDeath();
    }
    
    public delegate void ValueChangeEvent<T>(T diff); // FIX: T should be value type
    
    public static event ValueChangeEvent<int>? PlayerChangeHealth;
    private static int _playerHealthDiff;
    public static void PublishPlayerChangeHealth(int diff) { _playerHealthDiff += diff; }
    private static void BroadcastPlayerChangeHealth()
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
    private static void BroadcastBackgroundSetScrollSpeed() { 
        BackgroundSetScrollSpeed?.Invoke(_newBackgroundScroll, _newBackgroundScrollLerpTime);
    }

    public delegate void SimpleEvent();
    
    public static event SimpleEvent? PlayerDeath;
    private static bool _broadcastPlayerDeath;
    public static void PublishPlayerDeath() { _broadcastPlayerDeath = true; }
    private static void BroadcastPlayerDeath()
    {
        if (_broadcastPlayerDeath) PlayerDeath?.Invoke();
        _broadcastPlayerDeath = false;
    }
}