using invaders.enums;
using invaders.sceneobjects.renderobjects.gui;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using static SFML.Window.Keyboard.Key;
using static invaders.Utility;
namespace invaders.sceneobjects.renderobjects;

public sealed class Player : Actor
{
    private float _speed
    {
        get
        {
            float speed = 200f;
            if (_activePowerUps.ContainsKey(PowerUp.Types.ThrusterBoost)) speed += 100;
            return speed;
        }
    }

    // fire and burst rate 
    private const float _fireRate = 0.5f;
    private const float _burstRate = 0.1f;
    private float _fireTimer;
    private int _burstIndex;
    private int _burstLength
    {
        get
        {
            int length = 2;
            if (_activePowerUps.ContainsKey(PowerUp.Types.TripleShot)) length--;
            return length + _burstLengthLevel;
        }
    }

    
    // invincibility
    private const float _invicibilityWindow = 1f;
    private float _invincibilityTimer;
    
    // upgrade and power up
    private const float _bulletSpeedModifier = 20f;
    private const int _healthRegainAmount = 5;
    private int _bulletSpeedLevel;
    private int _defenseLevel;
    private int _burstLengthLevel;
    
    /// <summary>
    /// _activePowerUps contains all active power ups as keys and the time they've been alive as value
    /// </summary>
    private readonly Dictionary<PowerUp.Types, float> _activePowerUps = new();
    private const float _powerUpLifeTime = 6f;
    private readonly Dictionary<PowerUp.Types, SpriteGUI> _activePowerUpsSymbols = new()
    {
        { PowerUp.Types.RepairShip, new SpriteGUI(TextureRects["healthPower"]) },
        { PowerUp.Types.ThrusterBoost, new SpriteGUI(TextureRects["speedPower"]) },
        { PowerUp.Types.TripleShot, new SpriteGUI(TextureRects["triplePower"]) }
    };
    
    public Player() : base("invaders", TextureRects["player"], Scale)
    {
        maxHealth = 32;
        bulletDamage = 5;
        _invincibilityTimer = _invicibilityWindow;
        deathAnimationLength = 3f;
        zIndex = 10;
        InitPosition = Position = new Vector2f(
            (Program.ScreenWidth - Bounds.Width) / 2,
            Program.ScreenHeight - 120
        );
        hitSound.PlayingOffset = Time.FromMilliseconds(350);
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(40, 24);
    protected override float bulletSpeed => base.bulletSpeed + _bulletSpeedLevel * _bulletSpeedModifier;

    public override CollisionLayer Layer => CollisionLayer.Player;
    public override bool IsInvincible => _invincibilityTimer < _invicibilityWindow;

    public int CurrentHealth => currentHealth;

    
    protected override void Initialize()
    {
        base.Initialize();
        GlobalEventManager.PublishPlayerChangeHealth(maxHealth);
        animator.SetDefaultSprite(TextureRects["player"]);
        Animation invincible = new Animation("invincible", true, 25, _invicibilityWindow, blinkFrames);
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

        // XOR: if only one direction is held then move in that direction
        newPos.X = (right ^ left) switch
        {
            false => 0,
            true => right ? 1 : -1
        };
        newPos.Y = (up ^ down) switch
        {
            false => 0,
            true => down ? 1 : -1
        };

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
        
        CheckPowerUpIntersections();
        ProgressActivePowerUps(deltaTime);
        foreach (SpriteGUI symbol in _activePowerUpsSymbols.Values)
        {
            symbol.GetAnimatable().Animator.ProgressAnimation(deltaTime);
        }
    }

    public override void Render(RenderTarget target)
    {
        base.Render(target);
        
        List<SpriteGUI> powerUpActiveSprites = new();
        foreach (PowerUp.Types powerUp in PowerUp.StringToType.Values)
        {
            if (_activePowerUps.TryGetValue(powerUp, out float timer))
            {
                if (timer > _powerUpLifeTime - 2f)
                {
                    _activePowerUpsSymbols[powerUp].GetAnimatable().Animator.PlayAnimation("pulsing", false);
                }
                else
                {
                    // sets the the sprite to fully opaque if it was activated while it was pulsing
                    _activePowerUpsSymbols[powerUp].GetAnimatable().Animator.PlayAnimation("opaque", true);
                }
                powerUpActiveSprites.Add(_activePowerUpsSymbols[powerUp]);   
            }
        }

        
        // draw active sprites in bottom left corner
        float y = Program.ScreenHeight - _activePowerUpsSymbols[PowerUp.Types.RepairShip].Bounds.Height - 8;
        for (int i = 0; i < powerUpActiveSprites.Count; i++)
        {
            float x = 8 + (8 + powerUpActiveSprites[i].Bounds.Width) * i;
            powerUpActiveSprites[i].Position = new Vector2f(x, y);
            Animatable a = powerUpActiveSprites[i].GetAnimatable();
            if (a.Animator.IsAnimated) a.Animator.RenderAnimation(target);
            else target.Draw(a.Sprite);
        }        
    }

    private void ResetHealth()
    {
        GlobalEventManager.PublishPlayerChangeHealth(maxHealth - currentHealth);
        currentHealth = maxHealth;
    }

    protected override void Shoot(string type)
    {
        if (!_activePowerUps.ContainsKey(PowerUp.Types.TripleShot))
        {
            base.Shoot(type);
            return;
        }
        
        Bullet straightBullet = new(type, bulletSpeed, bulletDamage);
        straightBullet.Position = bulletOrigin;
        
        Bullet rightBullet = new(type, bulletSpeed, bulletDamage, (deltaTime, _, velocity) =>
        {
            // travel 30 degrees to the left
            return new Vector2f(-velocity.Y / 2f, velocity.Y).Normalized() * deltaTime * velocity.Length();
        });
        rightBullet.Position = bulletOrigin + new Vector2f(16, 0);
        
        Bullet leftBullet = new(type, bulletSpeed, bulletDamage, (deltaTime, _, velocity) =>
        {
            // travel 30 degrees to the right
            return new Vector2f(velocity.Y /2f, velocity.Y).Normalized() * deltaTime * velocity.Length();
        });
        leftBullet.Position = bulletOrigin - new Vector2f(16, 0);
        
        Scene.QueueSpawn(leftBullet);
        Scene.QueueSpawn(straightBullet);
        Scene.QueueSpawn(rightBullet);
        bulletSoundEffect.Play();
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
        int realDamage = (int) MathF.Max(damage - _defenseLevel, 1);
        currentHealth -= realDamage;
        _invincibilityTimer = 0f;
        animator.PlayAnimation("invincible", true);
        
        switch (currentHealth)
        {
            case > 0:
                hitSound.Play();
                break;
            case <= 0:
                Die();
                break;
        }

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
        adjustedPos.X = state.x switch
        {
            ScreenState.OutSideRight => Program.ScreenWidth - Bounds.Width - Settings.MarginSide,
            ScreenState.OutSideLeft => Settings.MarginSide,
            _ => adjustedPos.X
        };

        adjustedPos.Y = state.y switch
        {
            ScreenState.OutSideBottom => Program.ScreenHeight - Bounds.Height - Settings.MarginSide,
            ScreenState.OutSideTop => Settings.MarginSide + Settings.TopGuiHeight,
            _ => adjustedPos.Y
        };
    }

    private readonly new Animation.FrameRenderer[] blinkFrames =
    [
        (_, _) => { },
        (animatable, target) => target.Draw(animatable.Sprite)
    ];
    
    private void CheckPowerUpIntersections()
    {
        foreach (IntersectResult<PowerUp> intersect in this.FindIntersectingEntities<PowerUp>())
        {
            PowerUp.Types intersectedPowerUp = intersect.IntersectedEntity.Absorb();
            if (_activePowerUps.TryAdd(intersectedPowerUp, 0))
            {
                GainPowerUp(intersectedPowerUp);
            }
            else
            {
                Upgrade(intersectedPowerUp);
                _activePowerUps[intersectedPowerUp] = 0;
            }
        }
    }

    private void ProgressActivePowerUps(float deltaTime)
    {
        foreach (PowerUp.Types powerUp in _activePowerUps.Keys)
        {
            _activePowerUps[powerUp] += deltaTime;
            if (_activePowerUps[powerUp] >= _powerUpLifeTime)
            {
                _activePowerUps.Remove(powerUp);
            }
        }
    }
    
    private static void DrawPowerUpText(string message)
    {
        int length = message.Length;
        FadingTextGUI text = new FadingTextGUI(0.07f * length, message, 40, new Vector2f(0, -20));
        text.Position = MiddleOfScreen(text.Bounds, new Vector2f(0, -50));
        Scene.QueueSpawn(text);
    }
    
    private void GainPowerUp(PowerUp.Types powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUp.Types.RepairShip:
                RegainHealth(_healthRegainAmount);
                DrawPowerUpText("ship repaired");
                break;
            case PowerUp.Types.ThrusterBoost:
                DrawPowerUpText("thruster boost");
                break;
            case PowerUp.Types.TripleShot:
                DrawPowerUpText("triple fire");
                break;
            default: return;
        }
    }
    
    private void Upgrade(PowerUp.Types powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUp.Types.RepairShip:
                UpgradeDefense();
                RegainHealth(_healthRegainAmount);
                DrawPowerUpText("ship repaired\nhull reinforced");
                break;
            case PowerUp.Types.ThrusterBoost:
                UpgradeBulletSpeed();
                DrawPowerUpText("thruster boost\npower enhanced");
                break;
            case PowerUp.Types.TripleShot:
                UpgradeBurstIndex();
                DrawPowerUpText("triple fire\ncooling improved");
                break;
            default: return;
        }
    }

    private void UpgradeDefense() => _defenseLevel++;
    private void UpgradeBulletSpeed() => _bulletSpeedLevel++;
    private void UpgradeBurstIndex() => _burstLengthLevel++;
    
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
}