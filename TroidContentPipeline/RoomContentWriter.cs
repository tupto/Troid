using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = TroidEngine.World.Room;
using System.Text;
using TroidEngine.Entities;

namespace TroidContentPipeline
{
    [ContentTypeWriter]
    public class RoomContentWriter : ContentTypeWriter<TWrite>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "TroidEngine.ContentReaders.RoomContentReader, TroidEngine";
        }

        protected override void Write(ContentWriter output, TWrite value)
        {
			output.Write(value.Name);
            output.Write(value.Height);
            output.Write(value.Width);
			for (int y = 0; y < value.Height; y++)
			{
				for (int x = 0; x < value.Width; x++)
				{
					int id = value.GetTileID(x, y);

					output.Write(id);
					output.Write((int)value.GetTileCollision(x, y));
				}
			}

			int numDoors = 0;
			List<Door> doors = new List<Door>();
			foreach (Entity entity in value.GetEntities())
			{
				if (entity is Door)
				{
					numDoors++;
					Door de = entity as Door;
					doors.Add(de);
				}
			}

			output.Write(numDoors);
			foreach (Door de in doors)
			{
				output.Write((int)de.Position.X);
				output.Write((int)de.Position.Y);
				output.Write(de.Name ?? "door");
				output.Write(de.ConnectingRoomName ?? "room");
				output.Write(de.ConnectingDoorName ?? de.Name ?? "door");
			}
        }
    }
}
