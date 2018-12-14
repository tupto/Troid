using System.Runtime.Serialization;

namespace TroidRoomEditor
{
	[DataContract]
	public class RoomDataContract
	{
		[DataMember(Name = "tiles")]
		public TilesDataContract Tiles;
	}
}
