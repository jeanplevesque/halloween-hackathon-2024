using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Zombies;

public class Bullet
{
	private readonly float _length; // in pixels
	private readonly float _velocity; // in pixels per second
	private readonly Vector2 _textureScale;
	private readonly int _maxCollisionsCount;
	private readonly Texture2D _texture;
	private readonly BulletsComponent _bulletsComponent;
	private readonly Color _color;

	private float _remainingDuration; // in seconds
	private Vector2 _tipPosition;
	private Vector2 _tailPosition;
	private Vector2 _direction;

	public Bullet(
		float length,
		float velocity,
		int maxCollisionsCount,
		Texture2D texture,
		BulletsComponent bulletsComponent,
		float remainingDuration,
		Vector2 tailPosition,
		Vector2 direction,
		Color color)
	{
		_length = length;
		_velocity = velocity;
		_maxCollisionsCount = maxCollisionsCount;
		_texture = texture;
		_bulletsComponent = bulletsComponent;
		_remainingDuration = remainingDuration;
		_tailPosition = tailPosition;
		_direction = direction;
		_direction.Normalize();
		_color = color;
		_tipPosition = _tailPosition + _direction * _length;
		_textureScale = new Vector2(_length / _texture.Width, 2);
	}

	public bool IsAlive { get; private set; } = true;

	public void Update(float deltaTime)
	{
		if (!IsAlive)
		{
			return;
		}

		_tipPosition += _direction * _velocity * deltaTime;
		_tailPosition += _direction * _velocity * deltaTime;

		BounceOnTheEdges();
		_remainingDuration -= deltaTime;
		if (_remainingDuration <= 0)
		{
			IsAlive = false;
		}
	}

	private void BounceOnTheEdges()
	{
		if(_tipPosition.X < 0 || _tipPosition.X > _bulletsComponent.Bounds.Width)
		{
			// Bounce on the left or right edge
			_direction.X = -_direction.X;
		}
		if (_tipPosition.Y < 0 || _tipPosition.Y > _bulletsComponent.Bounds.Height)
		{
			// Bounce on the top or bottom edge
			_direction.Y = -_direction.Y;
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		if (!IsAlive)
		{
			return;
		}

		var angle = (float)Math.Atan2(_direction.Y, _direction.X);
		spriteBatch.Draw(
			texture: _texture,
			position: _tipPosition,
			sourceRectangle: null,
			color: _color,
			rotation: angle,
			scale: _textureScale,
			origin: Vector2.One * 0.5f,
			effects: SpriteEffects.None,
			layerDepth: 0);
	}

}
