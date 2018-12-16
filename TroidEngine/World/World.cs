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

		public void LoadRoom(string name, string door = null)
		{
			PlayerBase player = CurrentRoom.GetPlayer();
			CurrentRoom.RemoveEntity(player);

			CurrentRoom = rooms[name];
			CurrentRoom.AddEntity(player);

			if (door != null)
			{
				foreach (Entity entity in CurrentRoom.GetEntities())
				{
					if (entity is Door && entity.Name == door)
					{
						player.Position = entity.Position;

						if (player.Direction == Direction.Left)
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
		}

		public void Update(GameTime gameTime)
		{
			CurrentRoom.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			CurrentRoom.Draw(spriteBatch);
		}
	}
}
