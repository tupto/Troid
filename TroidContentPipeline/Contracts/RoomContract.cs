using System.Runtime.Serialization;

namespace TroidContentPipeline.Contracts
{
    [DataContract]
    public class RoomContract
    {
        [DataMember(Name = "tiles")]
        public TilesDataContract Tiles;
    }
}
