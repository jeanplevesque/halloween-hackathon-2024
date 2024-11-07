using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Zombies
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Player _player;

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
			_player = new Player(this, playerPosition);

			Components.Add(_player);
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
