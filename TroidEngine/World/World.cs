using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.Entities;

namespace TroidEngine.World
{
	public class World
	{
		public Room CurrentRoom;
		public bool DebugMode;

		private bool doingDoorTransition = false;
		private float doorTransitionTimer = 0.0f;
		private float doorTransitionMaxTime = 1.0f;
		private Vector2 doorTransitionPosDiff = Vector2.Zero;
		private Vector2 doorTranistionStartPos = Vector2.Zero;
		private Tile[,] doorTranistionTiles;

		private Dictionary<string, Room> rooms;

		public World()
		{
			rooms = new Dictionary<string, Room>();
		}

		public void AddRoom(Room room)
		{
			room.World = this;
			rooms.Add(room.Name ?? "unknown_room", room);

			foreach (Entity entity in room.GetEntities())
			{
				entity.World = this;
			}

			if (CurrentRoom == null)
			{
				CurrentRoom = room;
			}
		}

		private void BeginDoorTransition(Room room1, Room room2, Door door1, Door door2)
		{
			if (doingDoorTransition)
				return;

			doorTranistionStartPos = door1.Position;
			doorTransitionPosDiff = door2.Position - door1.Position;
			doingDoorTransition = true;
			doorTransitionTimer = 0.0f;

			Tile[] door1Tiles = new Tile[2];
			int x = (int)door1.Position.X / Tile.TILE_WIDTH;
			int y = (int)door1.Position.Y / Tile.TILE_HEIGHT;
			for (int i = 0; i < door1Tiles.Length; i++)
			{
				door1Tiles[i] = room1.Tiles[x, y + i];
			}

			Tile[] door2Tiles = new Tile[2];
			x = (int)door2.Position.X / Tile.TILE_WIDTH;
			y = (int)door2.Position.Y / Tile.TILE_HEIGHT;
			for (int i = 0; i < door2Tiles.Length; i++)
			{
				door2Tiles[i] = room2.Tiles[x, y + i];
			}

			doorTranistionTiles = new Tile[2, 2];
			if (doorTransitionPosDiff.X > 0)
			{
				doorTranistionTiles[0, 0] = door2Tiles[0];
				doorTranistionTiles[1, 0] = door1Tiles[0];
				doorTranistionTiles[0, 1] = door2Tiles[1];
				doorTranistionTiles[1, 1] = door1Tiles[1];
			}
			else
			{
				doorTranistionTiles[0, 0] = door1Tiles[0];
				doorTranistionTiles[1, 0] = door2Tiles[0];
				doorTranistionTiles[0, 1] = door1Tiles[1];
				doorTranistionTiles[1, 1] = door2Tiles[1];
			}
		}

		public void LoadRoom(string name, Door door = null)
		{
			PlayerBase player = CurrentRoom.GetPlayer();
			CurrentRoom.RemoveEntity(player);

			if (door != null)
			{
				foreach (Entity entity in rooms[name].GetEntities())
				{
					if (entity is Door && entity.Name == (door.ConnectingDoorName ?? door.Name))
					{
						BeginDoorTransition(CurrentRoom, rooms[name], door, (Door)entity);
						player.Position = entity.Position;

						if (player.Direction == Direction.Left || player.Velocity.X < 0)
						{
							player.Position.X -= player.Hitbox.Width;
						}
						else
						{
							player.Position.X += player.Hitbox.Width;
						}
					}
				}
			}

			CurrentRoom = rooms[name];
			CurrentRoom.AddEntity(player);
		}

		public void Update(GameTime gameTime)
		{
			if (doingDoorTransition)
			{
				doorTransitionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (doorTransitionTimer >= doorTransitionMaxTime)
				{
					doorTransitionTimer = 0.0f;
					doingDoorTransition = false;
				}
			}
			else
			{
				CurrentRoom.Update(gameTime);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (doingDoorTransition)
			{
				spriteBatch.GraphicsDevice.Clear(Color.Black);

				for (int x = 0; x < doorTranistionTiles.GetLength(0); x++)
				{
					for (int y = 0; y < doorTranistionTiles.GetLength(1); y++)
					{
						Vector2 currPos = doorTranistionStartPos + (doorTransitionPosDiff * (doorTransitionTimer / doorTransitionMaxTime)); 
						doorTranistionTiles[x, y].Draw(
							x + (int)currPos.X / Tile.TILE_WIDTH,
							y + (int)currPos.Y / Tile.TILE_WIDTH,
							spriteBatch
						);
					}
				}
			}
			else
			{
				CurrentRoom.Draw(spriteBatch);
			}
		}
	}
}
