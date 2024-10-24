using invaders.enums;
using SFML.Graphics;
using SFML.System;
using static invaders.Utility;

namespace invaders.sceneobjects.renderobjects;

public class Juggernaut : AbstractEnemy
{
    public override int ScoreValue => 1000;
    protected override float powerUpSpawnChance => 1f;

    private float _timeUntilFire;
    private float _fireTimer;
    private (bool left, bool right) _availableHands = (true, true);
    private readonly Sprite _leftHand = new();
    private readonly Sprite _rightHand = new();
    private const float _handRecoveryTime = 1f;
    private float _leftHandTimer;
    private float _rightHandTimer;
    private bool _lastShotLeftHand;

    public Juggernaut() : base("invaders", TextureRects["juggernaut"], Scale) {
        deathAnimationLength = 0.5f;
        maxHealth = 50;
        horizontalSpeed = 10;
        bulletDamage = 15;
        _leftHand.Texture = AssetManager.LoadTexture("invaders");
        _leftHand.TextureRect = TextureRects["juggernautBullet1"];
        _leftHand.Scale = new Vector2f(Scale, Scale);
        _leftHand.Origin =
            new Vector2f(_leftHand.GetGlobalBounds().Width / 2f, _leftHand.GetGlobalBounds().Height / 2f);
        _rightHand.Texture = AssetManager.LoadTexture("invaders");
        _rightHand.TextureRect = TextureRects["juggernautBullet2"];
        _rightHand.Scale = new Vector2f(Scale, Scale);
        _rightHand.Origin =
            new Vector2f(_rightHand.GetGlobalBounds().Width / 2f, _rightHand.GetGlobalBounds().Height / 2f);
    }

    protected override Vector2f bulletOrigin => Position + new Vector2f(36, 100);
    private Vector2f bulletOriginRight => bulletOrigin + new Vector2f(56, 0);
    protected override float bulletSpeed => 300f;

    protected override void Initialize() {
        _timeUntilFire = GetNewFireTime();

        animator.SetDefaultSprite(TextureRects["juggernaut"]);
        Animation idle = new("idle", false, 1, 0, idleFrames);
        animator.AddAnimation(idle);

        base.Initialize();
    }

    public override void Update(float deltaTime) {
        base.Update(deltaTime);

        if (WillDie) return;

        if (Position.Y < Settings.TopGuiHeight) return;

        _fireTimer += deltaTime;

        if (_fireTimer >= _timeUntilFire) {
            if (!_lastShotLeftHand) {
                _leftHandTimer = 0;
                _availableHands.left = false;
                _lastShotLeftHand = true;
                ShootHand(true);
            }
            else {
                _rightHandTimer = 0;
                _availableHands.right = false;
                _lastShotLeftHand = false;
                ShootHand(false);
            }

            _fireTimer = 0;
            _timeUntilFire = GetNewFireTime();
        }

        if (!_availableHands.left) _leftHandTimer += deltaTime;
        if (!_availableHands.right) _rightHandTimer += deltaTime;

        if (_leftHandTimer >= _handRecoveryTime) {
            _availableHands.left = true;
            _leftHandTimer = 0;
        }

        if (_rightHandTimer >= _handRecoveryTime) {
            _availableHands.right = true;
            _rightHandTimer = 0;
        }
    }

    private void ShootHand(bool left) {
        Bullet bullet;
        bullet = left
            ? new Bullet("juggernautLeft", bulletSpeed, bulletDamage)
            : new Bullet("juggernautRight", bulletSpeed, bulletDamage);
        bullet.Position = left ? bulletOrigin : bulletOriginRight;
        bullet.Layer = CollisionLayer.Enemy;
        bullet.Spawn();
        bulletSoundEffect.Play();
    }

    public override void Render(RenderTarget target) {
        base.Render(target);

        if (_availableHands.left) {
            _leftHand.Position = bulletOrigin;
            target.Draw(_leftHand);
        }

        if (_availableHands.right) {
            _rightHand.Position = bulletOriginRight;
            target.Draw(_rightHand);
        }
    }

    private static float GetNewFireTime() => 1.3f;

    private readonly Animation.FrameRenderer[] idleFrames = [
        (animatable, target) => { target.Draw(animatable.Sprite); },
    ];
}