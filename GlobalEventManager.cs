using invaders.sceneobjects;

namespace invaders;

public static class GlobalEventManager
{
    static GlobalEventManager()
    {
        Scene.GlobalEvents += BroadcastEvents;
    }
    
    public static void BroadcastEvents()
    {
        BroadcastPlayerChangeHealth();
        BroadcastBackgroundSetScrollSpeed();
        BroadcastPlayerDeath();
        BroadcastPlayerHit();
        BroadcastEnemyDeath();
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

    public static event SimpleEvent? PlayerHit;
    private static bool _broadcastPlayerHit;
    public static void PublishPlayerHit() { _broadcastPlayerHit = true; }
    private static void BroadcastPlayerHit()
    {
        if (_broadcastPlayerHit) PlayerHit?.Invoke();
        _broadcastPlayerHit = false;
    }
    

    public delegate void EnemyEvent(AbstractEnemy enemy);

    public static event EnemyEvent? EnemyDeath;
    private static List<AbstractEnemy> _deadEnemies = new();
    public static void PublishEnemyDead(AbstractEnemy enemy) { _deadEnemies.Add(enemy); }
    private static void BroadcastEnemyDeath()
    {
        foreach (AbstractEnemy enemy in _deadEnemies)
        {
            EnemyDeath?.Invoke(enemy);
        }
        _deadEnemies.Clear();
    }
}