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
	private readonly Texture2D _texture;
	private readonly BulletsComponent _bulletsComponent;
	private readonly Color _color;
	private readonly float _damage;

	private float _remainingDuration; // in seconds
	private Vector2 _tipPosition;
	private Vector2 _tailPosition;
	private Vector2 _direction;
	private int _remainingCollisionsCount;
	private Lazy<HashSet<Zombie>> _collidedZombies = new Lazy<HashSet<Zombie>>(() => new HashSet<Zombie>());

	public Bullet(
		float length,
		float velocity,
		int remainingCollisionsCount,
		Texture2D texture,
		BulletsComponent bulletsComponent,
		float remainingDuration,
		Vector2 tailPosition,
		Vector2 direction,
		Color color,
		float damage)
	{
		_length = length;
		_velocity = velocity;
		_remainingCollisionsCount = remainingCollisionsCount;
		_texture = texture;
		_bulletsComponent = bulletsComponent;
		_remainingDuration = remainingDuration;
		_tailPosition = tailPosition;
		_direction = direction;
		_direction.Normalize();
		_color = color;
		_damage = damage;
		_tipPosition = _tailPosition + _direction * _length;
		_textureScale = new Vector2(_length / _texture.Width, 2);
	}

	public bool IsAlive { get; private set; } = true;

	public float Damage => _damage;

	public Vector2 Position => _tipPosition;
	public Vector2 Direction => _direction;
	public float Velocity => _velocity;

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
		if (_tipPosition.X < 0 || _tipPosition.X > _bulletsComponent.Bounds.Width)
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

	public bool Intersects(Zombie zombie)
	{
		if (!IsAlive)
		{
			return false;
		}

		if (_collidedZombies.Value.Contains(zombie))
		{
			return false;
		}

		return CircleIntersectsLineSegment(_tailPosition, _tipPosition, zombie.Position, zombie.BoundingRadius);
	}

	public void Collide(Zombie zombie)
	{
		_collidedZombies.Value.Add(zombie);
		--_remainingCollisionsCount;
		if (_remainingCollisionsCount <= 0)
		{
			IsAlive = false;
		}
	}

	public static bool CircleIntersectsLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 circleCenter, float circleRadius)
	{
		Vector2 lineDir = lineEnd - lineStart;
		Vector2 toCircle = circleCenter - lineStart;

		// Project 'toCircle' onto the line direction vector
		float t = Vector2.Dot(toCircle, lineDir) / lineDir.LengthSquared();

		// Clamp t to the range [0, 1] to ensure projection point is within the segment
		t = MathHelper.Clamp(t, 0, 1);

		// Find the closest point on the segment to the circle center
		Vector2 closestPoint = lineStart + t * lineDir;

		// Check if the distance from the closest point to the circle center is within the radius
		return Vector2.DistanceSquared(closestPoint, circleCenter) <= circleRadius * circleRadius;
	}
}
