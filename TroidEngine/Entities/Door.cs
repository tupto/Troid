using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.World;

namespace TroidEngine.Entities
{
	public class Door : TriggerEntity
	{
		public string ConnectingRoomName;
		public string ConnectingDoorName;

		public Door(string name, Vector2 position)
			: base(name, position)
		{
			Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 16);
			ConnectingRoomName = null;
		}

		public override void OnEntityHit(Entity entity)
		{
			if (entity is PlayerBase)
			{
				World.LoadRoom(ConnectingRoomName, ConnectingDoorName);
			}
		}
	}
}
