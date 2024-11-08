using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombies;

public class ZombiesComponent : DrawableGameComponent
{
	const int SpawnPadding = 100; // Minimum distance from the screen edge where zombies can spawn.
	const float SpawnCooldown = 1.0f; // Delay between zombie spawns in seconds.

	private readonly Player _player;
	private readonly BulletsComponent _bullets;
	private readonly ParticlesComponent _particles;
	private readonly List<Zombie> _zombies = new List<Zombie>();
	private readonly List<SoundEffect> _splushSounds = new List<SoundEffect>();
	private SpriteFont _font;

	private float _remainingTimeBeforeNextSpawn = SpawnCooldown;

	public ZombiesComponent(Game game, Player player, BulletsComponent bullets, ParticlesComponent particles) : base(game)
	{
		_player = player;
		_bullets = bullets;
		_particles = particles;
	}

	public int Score { get; private set; }

	override protected void LoadContent()
	{
		for (int i = 0; i < 4; i++)
		{
			_splushSounds.Add(Game.Content.Load<SoundEffect>($"splush{i}"));
		}
		_font = Game.Content.Load<SpriteFont>("Font");
	}

	public override void Update(GameTime gameTime)
	{
		if (_remainingTimeBeforeNextSpawn <= 0)
		{
			Spawn();
		}
		else
		{
			_remainingTimeBeforeNextSpawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		foreach (var zombie in _zombies)
		{
			foreach (var bullet in _bullets.Bullets)
			{
				if (bullet.Intersects(zombie))
				{
					if (zombie.TakeDamage(bullet.Damage))
					{
						PlaySplushSound();
						++Score;
					}
					bullet.Collide(zombie);
					_particles.AddBloodSplash(bullet.Position, bullet.Direction, bullet.Velocity);
				}
			}

			if (zombie.CanHit() && zombie.Intersects(_player))
			{
				zombie.Hit();
				_player.TakeDamage(zombie.Damage);
			}

			zombie.Update(gameTime);
		}

		var zombiesToRemove = _zombies.Where(z => !z.IsAlive);
		if (zombiesToRemove.Any())
		{
			foreach (var zombie in zombiesToRemove.ToList())
			{
				_zombies.Remove(zombie);
			}
		}
	}

	private void PlaySplushSound()
	{
		var soundEffect = _splushSounds[Random.Shared.Next(0, _splushSounds.Count)];
		var soundInstance = soundEffect.CreateInstance();
		soundInstance.Volume = 0.8f;
		soundInstance.Play();
	}

	private void Spawn()
	{
		var kind = Random.Shared.Next(0, 10) switch // 10% chance to spawn a fast zombie.
		{
			< 3 => ZombieKind.Tank,
			< 5 => ZombieKind.Fast,
			_ => ZombieKind.Normal
		};

		var position = new Vector2(
			Random.Shared.Next(SpawnPadding, Game.GraphicsDevice.Viewport.Width - SpawnPadding),
			Random.Shared.Next(SpawnPadding, Game.GraphicsDevice.Viewport.Height - SpawnPadding)
		);

		var speed = kind switch
		{
			ZombieKind.Tank => 20,
			ZombieKind.Fast => 75,
			_ => 40
		};

		var health = kind switch
		{
			ZombieKind.Tank => 200,
			ZombieKind.Fast => 30,
			_ => 70
		};

		var damage = kind switch
		{
			ZombieKind.Tank => 20,
			ZombieKind.Fast => 5,
			_ => 10
		};

		var hitCooldown = kind switch
		{
			ZombieKind.Tank => 2,
			ZombieKind.Fast => 0.3f,
			_ => 1
		};

		var boundingRadius = kind switch
		{
			ZombieKind.Tank => 80,
			ZombieKind.Fast => 30,
			_ => 50
		};

		var zombie = new Zombie(
			speed: speed,
			texture: Game.Content.Load<Texture2D>("zombie0"),
			player: _player,
			position: position,
			remainingHealth: health,
			damage: damage,
			hitCooldown: hitCooldown,
			boundingRadius: boundingRadius / 2f
		);

		if (zombie.Intersects(_player))
		{
			Spawn();
			return;
		}

		_zombies.Add(zombie);
		_remainingTimeBeforeNextSpawn = SpawnCooldown;
	}

	public override void Draw(GameTime gameTime)
	{
		var spriteBatch = ((Game1)Game).SpriteBatch;
		foreach (var zombie in _zombies)
		{
			zombie.Draw(gameTime, spriteBatch);
		}
		// 500f1b
		var color = new Color(0x50, 0x0, 0x1b);
		spriteBatch.DrawString(
			_font,
			$"Score: {Score}",
			new Vector2(10, 10),
			color
		);
	}
}
