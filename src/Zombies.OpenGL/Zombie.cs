using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombies;

public class Zombie
{
	const float TargetRadius = 60.0f;

	private readonly float _speed;
	private readonly Texture2D _texture;
	private readonly Player _player;
	private readonly float _textureScale;
	private readonly float _boundingRadius;
	private readonly float _damage;
	private readonly float _hitCooldown; // In hits per second

	private Vector2 _position;
	private float _angle;
	private float _remainingHealth;
	private bool _isAlive = true;
	private float _remainingTimeBeforeCanHit;

	public Zombie(
		float speed,
		Texture2D texture,
		Player player,
		Vector2 position,
		float remainingHealth,
		float damage,
		float hitCooldown,
		float boundingRadius)
	{
		_speed = speed;
		_texture = texture;
		_player = player;
		_position = position;
		_remainingHealth = remainingHealth;
		_textureScale = TargetRadius / (float)_texture.Width;
		_hitCooldown = hitCooldown;
		_boundingRadius = boundingRadius;
	}

	public Vector2 Position => _position;
	public float BoundingRadius => _boundingRadius;
	public bool IsAlive => _isAlive;
	public float Damage => _damage;

	public void Update(GameTime gameTime)
	{
		var direction = _player.Position - _position;
		direction.Normalize();
		_position += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
		_angle = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			texture: _texture,
			position: _position,
			sourceRectangle: null,
			color: Color.White,
			rotation: _angle,
			scale: _textureScale,
			origin: Vector2.One * 0.5f * _texture.Width,
			effects: SpriteEffects.None,
			layerDepth: 0);
	}

	public void TakeDamage(object damage)
	{
		_remainingHealth -= (float)damage;
		if (_remainingHealth <= 0)
		{
			_isAlive = false;
		}
	}

	public bool CanHit()
	{
		return _hitCooldown <= 0;
	}

	public void Hit()
	{
		_remainingTimeBeforeCanHit = 1.0f / _hitCooldown;
	}

	public bool Intersects(Player player)
	{
		var distance = Vector2.Distance(_position, player.Position);
		return distance < _boundingRadius + Player.BoundingRadius;
	}
}
