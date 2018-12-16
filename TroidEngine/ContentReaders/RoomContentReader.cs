using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.World;

using TRead = TroidEngine.World.Room;

namespace TroidEngine.ContentReaders
{
	public class RoomContentReader : ContentTypeReader<TRead>
	{
		protected override TRead Read(ContentReader input, TRead existingInstance)
		{
			int height = input.ReadInt32();
			int width = input.ReadInt32();

			Room room = new TRead(width, height);
			room.Tiles = new Tile[width, height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					room.Tiles[x, y] = new Tile(input.ReadInt32());
					room.Tiles[x, y].CollisionType = (TileCollision)input.ReadInt32();
				}
			}

			return room;
		}
	}
}
