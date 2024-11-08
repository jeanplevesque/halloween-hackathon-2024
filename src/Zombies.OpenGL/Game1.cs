using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Zombies
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private BulletsComponent _bulletsComponent;
		private Player _player;
		private ZombiesComponent _zombiesComponent;

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
			_bulletsComponent = new BulletsComponent(this);
			_player = new Player(this, playerPosition, _bulletsComponent);
			_zombiesComponent = new ZombiesComponent(this, _player, _bulletsComponent);

			Components.Add(_bulletsComponent);
			Components.Add(_player);
			Components.Add(_zombiesComponent);
		}

		protected override void LoadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);


			_spriteBatch.Begin();
			base.Draw(gameTime);
			_spriteBatch.End();
		}
	}
}
