using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Zombies
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private ParticlesComponent _particlesComponent;
		private BulletsComponent _bulletsComponent;
		private Player _player;
		private ZombiesComponent _zombiesComponent;
		private SpriteFont _fontBig;
		private SpriteFont _font;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		public SpriteBatch SpriteBatch => _spriteBatch;

		protected override void Initialize()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			SetupGame();

			base.Initialize();
		}

		private void SetupGame()
		{
			var playerPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
			_particlesComponent = new ParticlesComponent(this);
			_bulletsComponent = new BulletsComponent(this);
			_player = new Player(this, playerPosition, _bulletsComponent);
			_zombiesComponent = new ZombiesComponent(this, _player, _bulletsComponent, _particlesComponent);

			Components.Add(_bulletsComponent);
			Components.Add(_player);
			Components.Add(_zombiesComponent);
			Components.Add(_particlesComponent);
		}

		private void RemoveComponents()
		{
			Components.Remove(_bulletsComponent);
			Components.Remove(_player);
			Components.Remove(_zombiesComponent);
			Components.Remove(_particlesComponent);
		}

		protected override void LoadContent()
		{
			_fontBig = Content.Load<SpriteFont>("FontBig");
			_font = Content.Load<SpriteFont>("Font");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (!_player.IsAlive)
			{
				if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
				{
					RemoveComponents();
					SetupGame();
				}
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			if (_player.IsAlive)
			{
				// 6d5b37
				var color = new Color(0x6d, 0x5b, 0x37, 0xff);
				GraphicsDevice.Clear(color);

				_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				base.Draw(gameTime);
				_spriteBatch.End();
			}
			else
			{
				// 6d5b37
				var color = new Color(0x50, 0x0, 0x1b);
				GraphicsDevice.Clear(color);

				_spriteBatch.Begin();
				_spriteBatch.DrawString(
					spriteFont: _fontBig,
					text: "DEAD",
					position: new Vector2(10, 10),
					color: Color.White);
				_spriteBatch.DrawString(
					spriteFont: _font,
					text: "Press Enter to restart",
					position: new Vector2(10, GraphicsDevice.Viewport.Height - 60),
					color: Color.White);
				_spriteBatch.End();
			}
		}
	}
}
