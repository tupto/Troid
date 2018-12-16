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
            output.Write(value.Height);
            output.Write(value.Width);
			int minonec = 0;
			for (int y = 0; y < value.Height; y++)
			{
				for (int x = 0; x < value.Width; x++)
				{
					int id = value.GetTileID(x, y);
					if (id == -1)
					{
						minonec++;
					}
					output.Write(id);
					output.Write((int)value.GetTileCollision(x, y));
				}
			}

			//throw new Exception("it was not 0 " + minonec + " times");
        }
    }
}
