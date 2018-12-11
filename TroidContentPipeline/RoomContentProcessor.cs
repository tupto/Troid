using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Troid.World;
using TroidContentPipeline.Contracts;

using TInput = TroidContentPipeline.Contracts.RoomContract;
using TOutput = Troid.World.Room;

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
        public override Room Process(TInput input, ContentProcessorContext context)
        {
            int width = input.Tiles.Width;
            int height = input.Tiles.Height;

            TOutput room = new Room(width, height);
            room.SetTiles(input.Tiles.Data);

            return room;
        }
    }
}