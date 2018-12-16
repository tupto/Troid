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

		private List<Room> rooms;

		public World()
		{
			rooms = new List<Room>();
		}

		public void AddRoom(Room room)
		{
			rooms.Add(room);

			if (CurrentRoom == null)
			{
				CurrentRoom = room;
			}
		}

		public void LoadRoom(int index)
		{
			PlayerBase player = CurrentRoom.GetPlayer();
			CurrentRoom.RemoveEntity(player);
			CurrentRoom = rooms[index];
			CurrentRoom.AddEntity(player);
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
