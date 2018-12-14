using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TroidEngine;
using TroidEngine.Graphics;
using TroidEngine.Grapihcs.UI;
using TroidEngine.World;

namespace TroidRoomEditor
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Texture2D pixel;

		List<Button> uiOptions;
		Button saveButton;
		Camera camera;
		World world;

		Rectangle currentMouseBlock;
		int currentTileIndex;
		bool bracketPressed = false;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			IsMouseVisible = true;

			world = new World();
			world.AddRoom(new Room(30, 20));

			saveButton = new Button(new Rectangle(5, 5, 100, 30), "Hello!");
			saveButton.OnClick += SaveButton_OnClick;

			currentTileIndex = 0;

			uiOptions = new List<Button>();
			uiOptions.Add(saveButton);

			camera = new Camera(world, GraphicsDevice.Viewport);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

            Tile.TileSheet = Content.Load<Texture2D>("tiles");
			Button.Font = Content.Load<SpriteFont>("freesans12");
			pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			pixel.SetData(new[] { Color.White });
			Button.Pixel = pixel;
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			MouseState ms = Mouse.GetState();
			KeyboardState ks = Keyboard.GetState();

			Vector2 gameLocation = camera.GetGameLocation(new Vector2(ms.X, ms.Y));
			int tileX = (int)gameLocation.X / Tile.TILE_WIDTH;
			int tileY = (int)gameLocation.Y / Tile.TILE_HEIGHT;
			bool buttonClicked = false;

			if (ks.IsKeyDown(Keys.OemOpenBrackets))
			{
				if (!bracketPressed)
				{
					currentTileIndex--;
					bracketPressed = true;

					if (currentTileIndex < 0)
						currentTileIndex = Tile.NumTiles - 1;
				}
			}
			else if (ks.IsKeyDown(Keys.OemCloseBrackets))
			{
				if (!bracketPressed)
				{
					currentTileIndex++;
					bracketPressed = true;

					if (currentTileIndex >= Tile.NumTiles)
						currentTileIndex = 0;
				}
			}
			else
			{
				bracketPressed = false;
			}

			foreach (Button btn in uiOptions)
			{
				if (btn.Bounds.Contains(ms.X, ms.Y))
				{
					if (ms.LeftButton == ButtonState.Pressed)
					{
						buttonClicked = true;
						btn.Click();
					}
				}
			}

			if (tileX >= world.CurrentRoom.Width)
				tileX = world.CurrentRoom.Width - 1;

			if (tileX < 0)
				tileX = 0;

			if (tileY >= world.CurrentRoom.Height)
				tileY = world.CurrentRoom.Height - 1;

			if (tileY < 0)
				tileY = 0;

			if (!buttonClicked)
			{
				if (ms.LeftButton == ButtonState.Pressed)
				{
					world.CurrentRoom.Tiles[tileX, tileY] = new Tile(currentTileIndex);
				}
			}

			currentMouseBlock = world.CurrentRoom.GetTileBounds(tileX, tileY);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetTransform());

			world.Draw(spriteBatch);
			DrawBorder(currentMouseBlock, 1, Color.Magenta);

			spriteBatch.End();

			spriteBatch.Begin();

			foreach (Button btn in uiOptions)
			{
				btn.Draw(spriteBatch);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
		{
			// Draw top line
			spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

			// Draw left line
			spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

			// Draw right line
			spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
											rectangleToDraw.Y,
											thicknessOfBorder,
											rectangleToDraw.Height), borderColor);
			// Draw bottom line
			spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
											rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
											rectangleToDraw.Width,
											thicknessOfBorder), borderColor);
		}

		void SaveButton_OnClick()
		{
			RoomDataContract rc = new RoomDataContract();
			rc.Tiles = new TilesDataContract();

			rc.Tiles.Width = world.CurrentRoom.Width;
			rc.Tiles.Height = world.CurrentRoom.Height;

			rc.Tiles.Data = new int[rc.Tiles.Width * rc.Tiles.Height];
			for (int i = 0; i < rc.Tiles.Data.Length; i++)
			{
				int x = i % rc.Tiles.Width;
				int y = i / rc.Tiles.Width;

				rc.Tiles.Data[i] = world.CurrentRoom.GetTileID(x, y);
			}

			using (MemoryStream ms = new MemoryStream())
			{
				string json = null;
				try
				{
					DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RoomDataContract), new DataContractJsonSerializerSettings
					{
						DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy-MM-dd HH:mm:ss")
					});
					serializer.WriteObject(ms, rc);

					byte[] jsonBytes = ms.ToArray();
					json = Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length);
				}
				catch (Exception e) { }

				if (!Directory.Exists("Rooms"))
				{
					Directory.CreateDirectory("Rooms");
				}

				FileStream room = File.Create(@"Rooms/new_room.room");
				byte[] bytes = Encoding.UTF8.GetBytes(json);
				room.Write(bytes, 0, bytes.Length);
				room.Close();
			}
		}
	}
}
