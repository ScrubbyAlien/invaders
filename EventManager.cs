using invaders.entity;

namespace invaders;

public static class EventManager
{
    public static void BroadcastEvents()
    {
        BroadcastPlayerChangeHealth();
    }
    
    public delegate void ValueChangeEvent<T>(T diff); // FIX: T should be value type
    
    public static event ValueChangeEvent<int>? PlayerChangeHealth;
    private static int playerHealthDiff;
    public static void PublishPlayerChangeHealth(int diff) { playerHealthDiff += diff; }
    public static void BroadcastPlayerChangeHealth()
    {
        PlayerChangeHealth?.Invoke(playerHealthDiff);
        playerHealthDiff = 0;
    }
}