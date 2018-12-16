using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using TroidEngine.World;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using TroidContentPipeline.Contracts;

using TImport = TroidEngine.ContentReaders.Contracts.RoomDataContract;

namespace TroidContentPipeline
{
    /// <summary>
    /// Used for importing Rooms to Troid
    /// </summary>
    [ContentImporter(".room", DisplayName = "Troid Room Importer - Troid", DefaultProcessor = "RoomContentProcessor")]
    public class RoomContentImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing JSON file: {0}", filename);

            TImport roomContract = default(TImport);

			string fileData = null;
			using (var fileStream = new FileStream(filename, FileMode.Open))
            {
				using (var streamReader = new StreamReader(fileStream))
				{
					fileData = streamReader.ReadToEnd();
				}
            }

			using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(fileData)))
			{
				try
                {
                    DataContractJsonSerializer dataSerializer = new DataContractJsonSerializer(typeof(RoomDataContract));
					roomContract = (TImport)dataSerializer.ReadObject(memoryStream);
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
