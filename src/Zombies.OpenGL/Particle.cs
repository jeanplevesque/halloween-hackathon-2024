using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Zombies;

public class Particle
{
	private readonly Texture2D _texture;
	private readonly float _textureScale;
	private readonly float _damping = 0.5f; // Multiplicator of velocity per second.
	private readonly float _angularDamping = 0.5f; // Multiplicator of angular velocity per second.

	private Vector2 _position;
	private Vector2 _velocity;
	private float _angle;
	private float _angularVelocity;
	private float _remainingDuration = 1f;
	private float _fadeOutDuration = 0.3f;
	private bool _isAlive = true;
	private float _alpha = 1f;

	public Particle(
		Texture2D texture,
		float textureScale,
		float damping,
		float angularDamping,
		Vector2 position,
		Vector2 velocity,
		float angle,
		float angularVelocity,
		float remainingDuration,
		float fadeOutDuration)
	{
		_texture = texture;
		_textureScale = textureScale;
		_damping = damping;
		_angularDamping = angularDamping;
		_position = position;
		_velocity = velocity;
		_angle = angle;
		_angularVelocity = angularVelocity;
		_remainingDuration = remainingDuration;
		_fadeOutDuration = fadeOutDuration;
	}

	public bool IsAlive => _isAlive;

	public void Update(float deltaTime)
	{
		_remainingDuration -= deltaTime;
		if (_remainingDuration <= 0)
		{
			_isAlive = false;
			return;
		}

		if (_remainingDuration < _fadeOutDuration)
		{
			_alpha = _remainingDuration / _fadeOutDuration;
		}

		_velocity *= (float)Math.Pow(_damping, deltaTime);
		_position += _velocity * deltaTime;
		_angularVelocity *= (float)Math.Pow(_angularDamping, deltaTime);
		_angle += _angularVelocity * deltaTime;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			texture: _texture,
			position: _position,
			sourceRectangle: null,
			color: Color.White * _alpha,
			rotation: _angle,
			scale: _textureScale,
			origin: Vector2.One * 0.5f * _texture.Width,
			effects: SpriteEffects.None,
			layerDepth: 0);
	}
}
