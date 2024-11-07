﻿using Microsoft.Xna.Framework;
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
	const float Speed = 80.0f;
	const float TargetRadius = 60.0f;

	private Vector2 _position;
	private Texture2D _texture;
	private SpriteBatch _spriteBatch;
	private Vector2 _toPointer;
	private float _angle;
	private float _textureScale;

	public Player(Game game, Vector2 position) : base(game)
	{
		_position = position;
	}

	protected override void LoadContent()
	{
		_spriteBatch = ((Game1)Game).SpriteBatch;
		_texture = Game.Content.Load<Texture2D>("player");
		_textureScale = TargetRadius / (float)_texture.Width;
	}

	public override void Update(GameTime gameTime)
	{
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
}