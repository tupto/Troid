using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TroidEngine.ContentReaders.Contracts
{
    [DataContract]
    public class RoomDataContract
    {
		[DataMember(Name = "name")]
		public string Name;

        [DataMember(Name = "width")]
		public int Width;

		[DataMember(Name = "height")]
		public int Height;

		[DataMember(Name = "data")]
		public TileDataContract[] Data;

		[DataMember(Name = "doors")]
		public List<DoorDataContract> Doors;
    }
}
