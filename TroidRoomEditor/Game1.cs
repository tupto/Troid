using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TroidEngine;
using TroidEngine.ContentReaders.Contracts;
using TroidEngine.Graphics;
using TroidEngine.Graphics.UI;
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

		UIManager uiManager;
		Camera camera;
		World world;

		Rectangle currentMouseBlock;
		int currentTileIndex;
		TileCollision currentCollisionMode;
		bool bracketPressed = false;

		bool collisionViewPressed = false;

		bool editCollisionsMode = false;

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

			currentCollisionMode = TileCollision.None;

			Button saveButton;
			Button loadButton;
			TextBox textBox;
			Icon currentTileIcon;

			textBox = new TextBox("textBox", new Rectangle(graphics.PreferredBackBufferWidth - 255,
												graphics.PreferredBackBufferHeight - 35,
												100, 30));
			textBox.Text = "cool_room";

			saveButton = new Button("saveBtn", new Rectangle(graphics.PreferredBackBufferWidth - 85,
			                                      graphics.PreferredBackBufferHeight - 35,
			                                      60, 30), "Save");
			saveButton.OnClick += SaveButton_OnClick;

			loadButton = new Button("loadBtn", new Rectangle(graphics.PreferredBackBufferWidth - 150,
												  graphics.PreferredBackBufferHeight - 35,
												  60, 30), "Load");
			loadButton.OnClick += LoadButton_OnClick;

			currentTileIcon = new Icon("tileIcn", new Rectangle(graphics.PreferredBackBufferWidth - 15,
												 graphics.PreferredBackBufferHeight - 15,
												 10, 10), Tile.TileSheet, null);

			currentTileIndex = 0;

			uiManager = new UIManager(this);
			uiManager.AddComponent(saveButton);
			uiManager.AddComponent(loadButton);
			uiManager.AddComponent(textBox);
			uiManager.AddComponent(currentTileIcon);

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
			UIComponent.Font = Content.Load<SpriteFont>("freesans12");
			pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			pixel.SetData(new[] { Color.White });
			UIComponent.Pixel = pixel;

			((Icon)uiManager.GetComponent("tileIcn")).IconSheet = Tile.TileSheet;
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			uiManager.Update(gameTime);

			MouseState ms = Mouse.GetState();
			KeyboardState ks = Keyboard.GetState();

			Vector2 gameLocation = camera.GetGameLocation(new Vector2(ms.X, ms.Y));
			int tileX = (int)gameLocation.X / Tile.TILE_WIDTH;
			int tileY = (int)gameLocation.Y / Tile.TILE_HEIGHT;

			if (!uiManager.UIInUse)
			{
				if (ks.IsKeyDown(Keys.OemOpenBrackets))
				{
					if (!bracketPressed)
					{
						if (!editCollisionsMode)
						{
							currentTileIndex--;
							bracketPressed = true;

							if (currentTileIndex < 0)
								currentTileIndex = Tile.NumTiles - 1;

							((Icon)uiManager.GetComponent("tileIcn")).SourceRectangle = Tile.GetSourceRect(currentTileIndex);
						}
						else
						{
							bracketPressed = true;
							var numCollisionModes = System.Enum.GetValues(typeof(TileCollision)).Length;
							currentCollisionMode++;
							if ((int)currentCollisionMode >= numCollisionModes)
								currentCollisionMode = 0;
						}
					}
				}
				else if (ks.IsKeyDown(Keys.OemCloseBrackets))
				{
					if (!bracketPressed)
					{
						if (!editCollisionsMode)
						{
							currentTileIndex++;
							bracketPressed = true;

							if (currentTileIndex >= Tile.NumTiles)
								currentTileIndex = 0;

							((Icon)uiManager.GetComponent("tileIcn")).SourceRectangle = Tile.GetSourceRect(currentTileIndex);
						}
						else
						{
							bracketPressed = true;
							var numCollisionModes = System.Enum.GetValues(typeof(TileCollision)).Length;
							currentCollisionMode--;
							if ((int)currentCollisionMode < 0)
								currentCollisionMode = (TileCollision)numCollisionModes - 1;
						}
					}
				}
				else
				{
					bracketPressed = false;
				}

				if (ks.IsKeyDown(Keys.C))
				{
					if (!collisionViewPressed)
					{
						collisionViewPressed = true;
						editCollisionsMode = !editCollisionsMode;
					}
				}
				else
				{
					collisionViewPressed = false;
				}

				if (tileX >= world.CurrentRoom.Width)
					tileX = world.CurrentRoom.Width - 1;

				if (tileX < 0)
					tileX = 0;

				if (tileY >= world.CurrentRoom.Height)
					tileY = world.CurrentRoom.Height - 1;

				if (tileY < 0)
					tileY = 0;

				if (!uiManager.UIClicked)
				{
					if (!editCollisionsMode)
					{
						if (ms.LeftButton == ButtonState.Pressed)
						{
							world.CurrentRoom.Tiles[tileX, tileY] = new Tile(currentTileIndex);
						}
						else if (ms.RightButton == ButtonState.Pressed)
						{
							world.CurrentRoom.Tiles[tileX, tileY] = null;
						}
					}
					else
					{
						if (ms.LeftButton == ButtonState.Pressed)
						{
							if (world.CurrentRoom.Tiles[tileX, tileY] != null)
								world.CurrentRoom.Tiles[tileX, tileY].CollisionType = currentCollisionMode;
						}
					}
				}

				currentMouseBlock = world.CurrentRoom.GetTileBounds(tileX, tileY);
			}

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

			if (editCollisionsMode)
			{
				world.CurrentRoom.DrawCollisionBoxes(spriteBatch, DrawBorder);
			}

			spriteBatch.End();

			spriteBatch.Begin();

			uiManager.Draw(spriteBatch);

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
			TroidEngine.ContentReaders.Contracts.RoomDataContract rc = new TroidEngine.ContentReaders.Contracts.RoomDataContract();
			rc.Data = new TileDataContract[world.CurrentRoom.Width * world.CurrentRoom.Height];

			rc.Width = world.CurrentRoom.Width;
			rc.Height = world.CurrentRoom.Height;

			for (int i = 0; i < rc.Data.Length; i++)
			{
				int x = i % rc.Width;
				int y = i / rc.Width;

				rc.Data[i] = new TileDataContract();
				rc.Data[i].ID = world.CurrentRoom.GetTileID(x, y);
				rc.Data[i].CollisionType = (int)world.CurrentRoom.GetTileCollision(x, y);
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

				FileStream room = File.Create(@"Rooms/" + uiManager.GetComponent("textBox").Text + ".room");
				byte[] bytes = Encoding.UTF8.GetBytes(json);
				room.Write(bytes, 0, bytes.Length);
				room.Close();
			}
		}

		public void LoadButton_OnClick()
		{
			string fileName = "Rooms/" + uiManager.GetComponent("textBox").Text + ".room";
			if (File.Exists("Rooms/" + uiManager.GetComponent("textBox").Text + ".room"))
			{
				TroidEngine.ContentReaders.Contracts.RoomDataContract roomContract = default(TroidEngine.ContentReaders.Contracts.RoomDataContract);
				string fileData = null;
				using (var fileStream = new FileStream(fileName, FileMode.Open))
				{
					using (var streamReader = new StreamReader(fileStream))
					{
						fileData = streamReader.ReadToEnd();
					}
				}

				using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(fileData)))
				{
					try
					{
						DataContractJsonSerializer dataSerializer = new DataContractJsonSerializer(typeof(TroidEngine.ContentReaders.Contracts.RoomDataContract));
						roomContract = (TroidEngine.ContentReaders.Contracts.RoomDataContract)dataSerializer.ReadObject(memoryStream);
					}
					catch (SerializationException e)
					{
						return;
					}
				}

				int width = roomContract.Width;
				int height = roomContract.Height;

				Room room = new Room(width, height);
				if (width * height != roomContract.Data.Length)
					throw new ArgumentException("Data length must equal height * width");

				room.Tiles = new Tile[width, height];
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						if (roomContract.Data[x + y * width].ID == -1)
							continue;

						room.Tiles[x, y] = new Tile(roomContract.Data[x + y * width].ID);
						room.Tiles[x, y].CollisionType = (TileCollision)roomContract.Data[x + y * width].CollisionType;
					}
				}

				world.AddRoom(room);
				world.CurrentRoom = room;
			}
		}
	}
}
