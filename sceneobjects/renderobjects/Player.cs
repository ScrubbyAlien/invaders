using invaders.enums;
using invaders.sceneobjects.renderobjects.gui;
using SFML.Audio;
using SFML.System;
using static SFML.Window.Keyboard.Key;
using static invaders.Utility;
namespace invaders.sceneobjects.renderobjects;

public sealed class Player : Actor
{
    private const float _speed = 200f;
    private const float _fireRate = 0.5f;
    private float _fireTimer;
    private int _burstLength = 2;
    private int _burstIndex;
    private const float _burstRate = 0.1f;
    private const float _invicibilityWindow = 1f;
    private float _invincibilityTimer;
    private int _defense;

    private const int HealthRegainAmount = 5;
    private const int DefenseUpgradeAmount = 1;

    private readonly HashSet<PowerUp.Types> activePowerUps = new();
    private readonly Dictionary<PowerUp.Types, float> powerUpTimers = new();
    
    public Player() : base("invaders", TextureRects["player"], Scale)
    {
        maxHealth = 32;
        bulletDamage = 5;
        _invincibilityTimer = _invicibilityWindow;
        deathAnimationLength = 3f;
        zIndex = 10;
        InitPosition = Position = new Vector2f(
            (Program.ScreenWidth - Bounds.Width) / 2,
            (Program.ScreenHeight - 120)
        );
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(40, 24);

    public override CollisionLayer Layer => CollisionLayer.Player;
    public override bool IsInvincible => _invincibilityTimer < _invicibilityWindow;

    public int CurrentHealth => currentHealth;

    
    protected override void Initialize()
    {
        base.Initialize();
        GlobalEventManager.PublishPlayerChangeHealth(maxHealth);
        animator.SetDefaultSprite(TextureRects["player"]);
        Animation invincible = new Animation("invincible", true, 25, _invicibilityWindow, blinking);
        Animation explode = new Animation("explode", true, 2, deathAnimationLength, explosionFrames);
        animator.AddAnimation(invincible);
        animator.AddAnimation(explode);

        explosionSound.Volume = 50;
        
        SetBulletSoundEffect("player_shot");
        bulletSoundEffect.Volume = 25;
        bulletSoundEffect.PlayingOffset = Time.FromMilliseconds(100);
    }

    public override void Destroy()
    {
        Scene.QueueSpawn(new LevelInfo<bool>(CurrentHealth > 0, "won"));   
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        if (inDeathAnimation)
        {
            if (MathF.Truncate(timeSinceDeath * 100) % 50 == 0)
            {
                new Sound(explosionSound).Play();
            }

            return;
        }
        
        _fireTimer += deltaTime;
        _invincibilityTimer += deltaTime;
        
        Vector2f newPos = new();
        bool right = AreAnyKeysPressed([Right, D]);
        bool left = AreAnyKeysPressed([Left, A]);
        bool up = AreAnyKeysPressed([Up, W]);
        bool down = AreAnyKeysPressed([Down, S]);
        
        if (right) newPos.X = 1;
        if (left) newPos.X = -1;
        if (right && left) newPos.X = 0;
        if (up) newPos.Y = -1;
        if (down) newPos.Y = 1;
        if (up && down) newPos.Y = 0;

        if (newPos.X > 0) sprite.TextureRect = TextureRects["playerRight"];
        else if (newPos.X < 0) sprite.TextureRect = TextureRects["playerLeft"];
        else sprite.TextureRect = TextureRects["player"];
        
        TryMoveWithinBounds(
            newPos.Normalized() * _speed * deltaTime, 
            Settings.MarginSide,
            Settings.MarginSide,
            Settings.TopGuiHeight + Settings.MarginSide,
            Settings.MarginSide);

        Scene.FindByType(out Invasion? invasion);
        if (!invasion!.InTransition)
        {
            if (AreAnyKeysPressed([Space]))
            {
                if (_burstIndex == 0 && _fireTimer >= _fireRate)
                {
                    Shoot("player");
                    _burstIndex++;
                    _fireTimer = 0;
                } 
                else if (_burstIndex > 0 && _burstIndex < _burstLength && _fireTimer >= _burstRate)
                {
                    Shoot("player");
                    _burstIndex++;
                    _fireTimer = 0;
                }
            }

            if (_fireTimer >= _fireRate) _burstIndex = 0;
            
        }
        
        foreach (IntersectResult<PowerUp> intersect in this.FindIntersectingEntities<PowerUp>())
        {
            PowerUp.Types intersectedPowerUp = intersect.IntersectedEntity.Absorb();
            if (activePowerUps.Add(intersectedPowerUp))
            {
                GainPowerUp(intersectedPowerUp);
            }
            else
            {
                Upgrade(intersectedPowerUp);
                activePowerUps.Remove(intersectedPowerUp);
            }
        }
    }

    private void DrawPowerUpText(string message)
    {
        int lines = message.Split("\n").Length;
        FadingTextGUI text = new FadingTextGUI(0.7f * lines, message, 40);
        text.SetDrift(new Vector2f(0, -20));
        text.Position = MiddleOfScreen(text.Bounds, new Vector2f(0, -50));
        Scene.QueueSpawn(text);
    }
    
    private void GainPowerUp(PowerUp.Types powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUp.Types.RepairShip:
                RegainHealth(HealthRegainAmount);
                DrawPowerUpText("ship repaired");
                break;
            default: return;
        }
    }
    
    private void Upgrade(PowerUp.Types powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUp.Types.RepairShip:
                UpgradeDefense(DefenseUpgradeAmount);
                RegainHealth(HealthRegainAmount);
                DrawPowerUpText("ship repaired\nhull reinforced");
                break;
            // case PowerUp.Types.ThrusterBoost:
            default: return;
        }

        activePowerUps.Remove(powerUpType);
    }

    private void UpgradeDefense(int upgradeAmount)
    {
        _defense += upgradeAmount;
    }

    private void RegainHealth(int regain)
    {
        if (currentHealth + regain >= maxHealth)
        {
            ResetHealth();
        }
        else
        {
            currentHealth += regain;
            GlobalEventManager.PublishPlayerChangeHealth(regain);
        }
    }

    public void ResetHealth()
    {
        GlobalEventManager.PublishPlayerChangeHealth(maxHealth - currentHealth);
        currentHealth = maxHealth;
    }

    public override void HitByBullet(Bullet bullet)
    {
        if (!IsInvincible)
        {
            TakeDamage(bullet.Damage);
        }
    }
    
    protected override void TakeDamage(int damage)
    {
        // can't take less than one damage even if defense is really high
        int realDamage = (int) MathF.Max(damage - _defense, 1);
        currentHealth -= realDamage;
        _invincibilityTimer = 0f;
        animator.PlayAnimation("invincible", true);
        if (currentHealth > 0)
        {
            hitSound.PlayingOffset = Time.FromMilliseconds(350);
            hitSound.Play();
        }
        if (currentHealth <= 0) Die();
        GlobalEventManager.PublishPlayerHit();
        GlobalEventManager.PublishPlayerChangeHealth(-realDamage);
    }

    protected override void Die()
    {
        GlobalEventManager.PublishPlayerDeath();
        animator.PlayAnimation("explode", true);
        base.Die();
    }

    protected override void OnOutsideScreen((ScreenState x, ScreenState y) state, Vector2f outsidePos, out Vector2f adjustedPos)
    {
        adjustedPos = outsidePos;
        switch (state.x)
        {
            case ScreenState.OutSideRight:
                adjustedPos.X = Program.ScreenWidth - Bounds.Width - Settings.MarginSide;
                break;
            case ScreenState.OutSideLeft:
                adjustedPos.X = Settings.MarginSide;
                break;
        }

        switch (state.y)
        {
            case ScreenState.OutSideBottom:
                adjustedPos.Y = Program.ScreenHeight - Bounds.Height - Settings.MarginSide;
                break;
            case ScreenState.OutSideTop:
                adjustedPos.Y = Settings.MarginSide + Settings.TopGuiHeight;
                break;
        }
    }

    private Animation.FrameRenderer[] blinking =
    [
        (_, _) => { },
        (animatable, target) =>
        {
            target.Draw(animatable.Sprite);
        },
    ];
}