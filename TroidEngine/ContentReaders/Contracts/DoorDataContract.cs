using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TroidEngine.ContentReaders.Contracts
{
	[DataContract]
	public class DoorDataContract
	{
		[DataMember(Name = "name")]
		public string Name;

		[DataMember(Name = "x")]
		public int X;

		[DataMember(Name = "y")]
		public int Y;

		[DataMember(Name = "connecting_room_name")]
		public string ConnectingRoomName;

		[DataMember(Name = "connecting_door_name")]
		public string ConnectingDoorName;
	}
}
