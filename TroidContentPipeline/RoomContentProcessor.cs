using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using TroidEngine.ContentReaders.Contracts;
using TroidEngine.World;
using TInput = TroidEngine.ContentReaders.Contracts.RoomDataContract;
using TOutput = TroidEngine.World.Room;

namespace TroidContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Troid Room Processor - Troid")]
    public class RoomContentProcessor : ContentProcessor<TInput, TOutput>
    {
		public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            int width = input.Width;
            int height = input.Height;

            TOutput room = new TOutput(width, height);
			room.Name = input.Name;

			if (width * height != input.Data.Length)
				throw new ArgumentException("Data length must equal height * width");

			room.Tiles = new Tile[width, height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (input.Data[x + y * width].ID == -1)
						continue;

					room.Tiles[x, y] = new Tile(input.Data[x + y * width].ID);
					room.Tiles[x, y].CollisionType = (TileCollision)input.Data[x + y * width].CollisionType;
				}
			}

			foreach (DoorDataContract door in input.Doors)
			{
				TroidEngine.Entities.Door de = new TroidEngine.Entities.Door(
					door.Name, new Microsoft.Xna.Framework.Vector2(door.X, door.Y)
				);
				de.ConnectingRoomName = door.ConnectingRoomName;
				de.ConnectingDoorName = door.ConnectingDoorName;

				room.AddEntity(de);
			}

            return room;
        }
    }
}