using System.Runtime.Serialization;

namespace TroidEngine.ContentReaders.Contracts
{
    [DataContract]
    public class TileDataContract
    {
		[DataMember(Name = "id")]
		public int ID;

		[DataMember(Name = "collision_type")]
		public int CollisionType;
    }
}
