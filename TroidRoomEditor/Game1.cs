﻿using System;
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
using TroidEngine.Entities;
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

		Vector2 doorPlacePosition;
		bool prompting;

		bool editModeCyclePressed = false;

		EditMode editMode = EditMode.Tile;

		private enum EditMode
		{
			Tile, Collision, Door
		}

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
			Room room = new Room(30, 20);
			room.Name = "new_room";
			world.AddRoom(room);

			currentCollisionMode = TileCollision.None;

			Button saveButton;
			Button loadButton;
			TextBox textBox;
			Icon currentTileIcon;
			Label modeLabel;
			Box doorPromptBox;
			TextBox connectingRoomText;
			TextBox connectingDoorNameText;

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

			modeLabel = new Label("modeLbl", new Rectangle(graphics.PreferredBackBufferWidth - 115,
			                                               5, 100, 30), editMode.ToString());

			doorPromptBox = new Box("doorPromptBox", new Rectangle(0, 0,
			                                               graphics.PreferredBackBufferWidth,
			                                               graphics.PreferredBackBufferHeight));
			doorPromptBox.Visible = false;

			connectingDoorNameText = new TextBox("connectingDoorNameTxt", new Rectangle(
												graphics.PreferredBackBufferWidth / 2 - 75,
												graphics.PreferredBackBufferHeight / 2 - 15, 150, 30));
			connectingDoorNameText.Text = "door_name";
			connectingDoorNameText.Visible = false;
			connectingDoorNameText.OnEnter += PromptTextBox_OnEnter;

			connectingRoomText = new TextBox("connectToRoomTxt", new Rectangle(
												graphics.PreferredBackBufferWidth / 2 - 75,
												graphics.PreferredBackBufferHeight / 2 - 50, 150, 30));
			connectingRoomText.Text = "room_name";
			connectingRoomText.Visible = false;

			currentTileIndex = 0;

			uiManager = new UIManager(this);
			uiManager.AddComponent(saveButton);
			uiManager.AddComponent(loadButton);
			uiManager.AddComponent(textBox);
			uiManager.AddComponent(currentTileIcon);
			uiManager.AddComponent(modeLabel);
			uiManager.AddComponent(doorPromptBox);
			uiManager.AddComponent(connectingDoorNameText);
			uiManager.AddComponent(connectingRoomText);

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
						if (editMode == EditMode.Tile)
						{
							currentTileIndex--;
							bracketPressed = true;

							if (currentTileIndex < 0)
								currentTileIndex = Tile.NumTiles - 1;

							((Icon)uiManager.GetComponent("tileIcn")).SourceRectangle = Tile.GetSourceRect(currentTileIndex);
						}
						else if (editMode == EditMode.Collision)
						{
							bracketPressed = true;
							var numCollisionModes = System.Enum.GetValues(typeof(TileCollision)).Length;
							currentCollisionMode++;
							if ((int)currentCollisionMode >= numCollisionModes)
								currentCollisionMode = 0;
						}
						else if (editMode == EditMode.Door)
						{
						}
					}
				}
				else if (ks.IsKeyDown(Keys.OemCloseBrackets))
				{
					if (!bracketPressed)
					{
						if (editMode == EditMode.Tile)
						{
							currentTileIndex++;
							bracketPressed = true;

							if (currentTileIndex >= Tile.NumTiles)
								currentTileIndex = 0;

							((Icon)uiManager.GetComponent("tileIcn")).SourceRectangle = Tile.GetSourceRect(currentTileIndex);
						}
					}
				}
				else
				{
					bracketPressed = false;
				}

				if (ks.IsKeyDown(Keys.C))
				{
					if (!editModeCyclePressed)
					{
						editModeCyclePressed = true;

						var numCollisionModes = Enum.GetValues(typeof(EditMode)).Length;
						editMode++;
						if ((int)editMode >= numCollisionModes)
							editMode = 0;

						uiManager.GetComponent("modeLbl").Text = editMode.ToString();
					}
				}
				else
				{
					editModeCyclePressed = false;
				}

				if (tileX >= world.CurrentRoom.Width)
					tileX = world.CurrentRoom.Width - 1;

				if (tileX < 0)
					tileX = 0;

				if (tileY >= world.CurrentRoom.Height)
					tileY = world.CurrentRoom.Height - 1;

				if (tileY < 0)
					tileY = 0;

				if (!uiManager.UIInUse && !uiManager.UIClicked)
				{
					if (editMode == EditMode.Tile)
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
					else if (editMode == EditMode.Collision)
					{
						if (ms.LeftButton == ButtonState.Pressed)
						{
							if (world.CurrentRoom.Tiles[tileX, tileY] != null)
								world.CurrentRoom.Tiles[tileX, tileY].CollisionType = currentCollisionMode;
						}
					}
					else if (editMode == EditMode.Door)
					{
						if (ms.LeftButton == ButtonState.Pressed && !uiManager.GetComponent("doorPromptBox").Visible)
						{
							uiManager.GetComponent("doorPromptBox").Visible = true;
							uiManager.GetComponent("connectToRoomTxt").Visible = true;
							uiManager.GetComponent("connectingDoorNameTxt").Visible = true;
							doorPlacePosition = new Vector2(tileX * Tile.TILE_WIDTH, tileY * Tile.TILE_HEIGHT);
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

			if (editMode == EditMode.Collision)
			{
				world.CurrentRoom.DrawCollisionBoxes(spriteBatch, DrawBorder);
			}
			else if (editMode == EditMode.Door)
			{
				foreach (Entity entity in world.CurrentRoom.GetEntities())
				{
					DrawBorder(entity.Hitbox, 1, Color.Blue);
				}
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
			RoomDataContract rc = new RoomDataContract();
			rc.Data = new TileDataContract[world.CurrentRoom.Width * world.CurrentRoom.Height];

			rc.Width = world.CurrentRoom.Width;
			rc.Height = world.CurrentRoom.Height;
			rc.Name = uiManager.GetComponent("textBox").Text;

			for (int i = 0; i < rc.Data.Length; i++)
			{
				int x = i % rc.Width;
				int y = i / rc.Width;

				rc.Data[i] = new TileDataContract();
				rc.Data[i].ID = world.CurrentRoom.GetTileID(x, y);
				rc.Data[i].CollisionType = (int)world.CurrentRoom.GetTileCollision(x, y);
			}

			rc.Doors = new List<DoorDataContract>();
			foreach (Entity entity in world.CurrentRoom.GetEntities())
			{
				if (entity is Door)
				{
					Door door = entity as Door;

					DoorDataContract ddc = new DoorDataContract();
					ddc.X = (int)door.Position.X;
					ddc.Y = (int)door.Position.Y;
					ddc.Name = door.Name;
					ddc.ConnectingRoomName = door.ConnectingRoomName;
					ddc.ConnectingDoorName = door.ConnectingDoorName;

					rc.Doors.Add(ddc);
				}
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

		void PromptTextBox_OnEnter()
		{
			uiManager.GetComponent("doorPromptBox").Visible = false;
			uiManager.GetComponent("connectToRoomTxt").Visible = false;
			uiManager.GetComponent("connectingDoorNameTxt").Visible = false;

			Door door = new Door(uiManager.GetComponent("connectingDoorNameTxt").Text, doorPlacePosition);
			door.ConnectingRoomName = uiManager.GetComponent("connectToRoomTxt").Text;

			world.CurrentRoom.AddEntity(door);
		}
	}
}
