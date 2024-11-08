using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombies;

public class BulletsComponent : DrawableGameComponent
{
	private readonly List<Bullet> _bullets = new List<Bullet>();

	public BulletsComponent(Game game) : base(game)
	{
		BulletTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
		BulletTexture.SetData<Color>(new Color[1] { Color.White });
		Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
	}

	public Texture2D BulletTexture { get; private set; }
	public Rectangle Bounds { get; private set; }

	public void AddBullet(Vector2 position, Vector2 direction)
	{
		var bullet = new Bullet(
			length: 20,
			velocity: 400,
			maxCollisionsCount: 1,
			texture: BulletTexture,
			bulletsComponent: this,
			remainingDuration: 3,
			tailPosition: position,
			direction: direction,
			Color.Yellow
		);

		_bullets.Add(bullet);
	}

	public override void Update(GameTime gameTime)
	{
		foreach (var bullet in _bullets)
		{
			bullet.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		var bulletsToRemove = _bullets.Where(b => !b.IsAlive);
		if (bulletsToRemove.Any())
		{
			foreach (var bullet in bulletsToRemove.ToList())
			{
				_bullets.Remove(bullet);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		var spriteBatch = ((Game1)Game).SpriteBatch;

		foreach (var bullet in _bullets)
		{
			bullet.Draw(spriteBatch);
		}
	}
}
