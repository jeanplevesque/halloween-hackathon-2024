﻿using Microsoft.Xna.Framework;
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
	private readonly List<Zombie> _zombies = new List<Zombie>();

	private float _remainingTimeBeforeNextSpawn = SpawnCooldown;

	public ZombiesComponent(Game game, Player player, BulletsComponent bullets) : base(game)
	{
		_player = player;
		_bullets = bullets;
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
					zombie.TakeDamage(bullet.Damage);
					bullet.Collide(zombie);
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

	private void Spawn()
	{
		var position = new Vector2(
			Random.Shared.Next(SpawnPadding, Game.GraphicsDevice.Viewport.Width - SpawnPadding),
			Random.Shared.Next(SpawnPadding, Game.GraphicsDevice.Viewport.Height - SpawnPadding)
		);

		var zombie = new Zombie(
			speed: 40,
			texture: Game.Content.Load<Texture2D>("zombie0"),
			player: _player,
			position: position,
			remainingHealth: 100,
			damage: 10,
			hitCooldown: 1,
			boundingRadius: 20
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
		foreach (var zombie in _zombies)
		{
			zombie.Draw(gameTime, ((Game1)Game).SpriteBatch);
		}
	}
}