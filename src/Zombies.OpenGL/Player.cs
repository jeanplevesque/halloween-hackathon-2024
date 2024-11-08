using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombies;

public class Player : DrawableGameComponent
{
	// In pixels per second
	const float Speed = 100.0f;
	const float TargetRadius = 60.0f;
	public const float BoundingRadius = 30.0f;

	private Vector2 _position;
	private readonly BulletsComponent _bullets;
	private Texture2D _texture;
	private SpriteBatch _spriteBatch;
	private Vector2 _toPointer;
	private float _angle;
	private float _textureScale;
	private float _remainingShootingCooldown;
	private float _remainingHealth = 100;
	private bool _isAlive = true;

	public Player(Game game, Vector2 position, BulletsComponent bullets) : base(game)
	{
		_position = position;
		_bullets = bullets;
	}

	public Vector2 Position => _position;

	protected override void LoadContent()
	{
		_spriteBatch = ((Game1)Game).SpriteBatch;
		_texture = Game.Content.Load<Texture2D>("player");
		_textureScale = TargetRadius / (float)_texture.Width;
	}

	public override void Update(GameTime gameTime)
	{
		if (!_isAlive)
		{
			return;
		}

		_remainingShootingCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		HandleInput(gameTime);
	}

	private void HandleInput(GameTime gameTime)
	{
		var direction = Vector2.Zero;
		var keyboardState = Keyboard.GetState();

		if (keyboardState.IsKeyDown(Keys.W))
		{
			direction.Y -= 1;
		}
		if (keyboardState.IsKeyDown(Keys.A))
		{
			direction.X -= 1;
		}
		if (keyboardState.IsKeyDown(Keys.S))
		{
			direction.Y += 1;
		}
		if (keyboardState.IsKeyDown(Keys.D))
		{
			direction.X += 1;
		}

		if (direction != Vector2.Zero)
		{
			direction.Normalize();
			_position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		var mouseState = Mouse.GetState();
		_toPointer = new Vector2(mouseState.X, mouseState.Y) - _position;
		_angle = (float)Math.Atan2(_toPointer.Y, _toPointer.X) + MathHelper.PiOver2;

		if (mouseState.LeftButton == ButtonState.Pressed && _remainingShootingCooldown <= 0)
		{
			_bullets.AddBullet(_position, _toPointer);
			_remainingShootingCooldown = 0.1f;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		_spriteBatch.Draw(
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

	public void TakeDamage(float damage)
	{
		_remainingHealth -= damage;
		if (_remainingHealth <= 0)
		{
			_isAlive = false;
		}
	}
}
