using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.World;

using TRead = TroidEngine.World.Room;
using TroidEngine.Entities;

namespace TroidEngine.ContentReaders
{
	public class RoomContentReader : ContentTypeReader<TRead>
	{
		protected override TRead Read(ContentReader input, TRead existingInstance)
		{
			string name = input.ReadString();
			int height = input.ReadInt32();
			int width = input.ReadInt32();

			Room room = new TRead(width, height);
			room.Tiles = new Tile[width, height];
			room.Name = name;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int tileID = input.ReadInt32();
					if (tileID != -1)
					{
						room.Tiles[x, y] = new Tile(tileID);
						room.Tiles[x, y].CollisionType = (TileCollision)input.ReadInt32();
					}
					else
					{
						input.ReadInt32();
					}
				}
			}

			int numDoors = input.ReadInt32();
			for (int i = 0; i < numDoors; i++)
			{
				int x = input.ReadInt32();
				int y = input.ReadInt32();
				string doorName = input.ReadString();
				string roomName = input.ReadString();
				string exitDoorName = input.ReadString();

				Door door = new Door(doorName, new Microsoft.Xna.Framework.Vector2(x, y));
				door.ConnectingRoomName = roomName;
				door.ConnectingDoorName = exitDoorName;

				room.AddEntity(door);
			}

			return room;
		}
	}
}
