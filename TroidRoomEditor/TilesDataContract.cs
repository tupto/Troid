using System.Runtime.Serialization;
namespace TroidRoomEditor
{
	[DataContract]
	public class TilesDataContract
	{
		[DataMember(Name = "width")]
		public int Width;

		[DataMember(Name = "height")]
		public int Height;

		[DataMember(Name = "data")]
		public int[] Data;
	}
}
