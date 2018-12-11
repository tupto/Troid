using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Troid.World;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using TroidContentPipeline.Contracts;

using TImport = TroidContentPipeline.Contracts.RoomContract;

namespace TroidContentPipeline
{
    /// <summary>
    /// Used for importing Rooms to Troid
    /// </summary>
    [ContentImporter(".room", DisplayName = "Room Data Importer", DefaultProcessor = "RoomContentProcessor")]
    public class RoomContentImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing JSON file: {0}", filename);

            RoomContract roomContract = default(RoomContract);

            using (var fileStream = new FileStream(filename, FileMode.Open))
            {
                try
                {
                    DataContractJsonSerializer dataSerializer = new DataContractJsonSerializer(typeof(RoomContract));
                    roomContract = (RoomContract)dataSerializer.ReadObject(fileStream);
                }
                catch (SerializationException e)
                {
                    context.Logger.LogImportantMessage("SerializationException thrown reading RoomContract: {0}", e.Message);
                    return null;
                }
            }

            return roomContract;
        }
    }
}
