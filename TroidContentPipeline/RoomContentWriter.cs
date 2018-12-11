using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = Troid.World.Room;
using Troid.World;

namespace TroidContentPipeline
{
    [ContentTypeWriter]
    public class RoomContentWriter : ContentTypeWriter<TWrite>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Troid.Utils.ContentReaders, Troid";
        }

        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.Height);
            output.Write(value.Width);

            for (int y = 0; y < value.Height; y++)
            {
                for (int x = 0; x < value.Width; x++)
                {
                    output.Write(value.GetTileID(x, y));
                }
            }
        }
    }
}
