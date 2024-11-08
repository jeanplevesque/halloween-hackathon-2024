using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombies;

public class ParticlesComponent : DrawableGameComponent
{
	private readonly List<Particle> _particles = new List<Particle>();

	public ParticlesComponent(Game game) : base(game)
	{
	}

	public void AddBloodSplash(Vector2 position, Vector2 direction, float velocity)
	{
		const int count = 10;
		for (int i = 0; i < count; i++)
		{
			var textureIndex = Random.Shared.Next(0, 5);
			var adjustedVelocity = (direction * velocity + NextVector2() * velocity * 0.5f) * NextFloat(0.1f, 0.9f);
			var particle = new Particle(
				texture: Game.Content.Load<Texture2D>($"blood{textureIndex}"),
				textureScale: NextFloat(0.25f, 0.75f),
				damping: NextFloat(0.3f, 0.6f),
				angularDamping: NextFloat(0.5f, 0.75f),
				position: position,
				velocity: adjustedVelocity,
				angle: NextFloat(0, MathHelper.TwoPi),
				angularVelocity: NextFloat(0, MathHelper.TwoPi),
				remainingDuration: NextFloat(0.5f, 1.5f),
				fadeOutDuration: 0.3f
			);

			_particles.Add(particle);
		}
	}

	public override void Update(GameTime gameTime)
	{
		for (int i = _particles.Count - 1; i >= 0; i--)
		{
			var particle = _particles[i];
			particle.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			if (!particle.IsAlive)
			{
				_particles.RemoveAt(i);
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		foreach (var particle in _particles)
		{
			particle.Draw(((Game1)Game).SpriteBatch);
		}
	}

	private static float NextFloat(float minValue, float maxValue)
	{
		return (float)Random.Shared.NextDouble() * (maxValue - minValue) + minValue;
	}

	private static Vector2 NextVector2()
	{
		return NextVector2((float)Random.Shared.NextDouble() * MathHelper.TwoPi);
	}

	private static Vector2 NextVector2(float angle)
	{
		var x = (float)Math.Cos(angle);
		var y = (float)Math.Sin(angle);
		return new Vector2(x, y);
	}
}
