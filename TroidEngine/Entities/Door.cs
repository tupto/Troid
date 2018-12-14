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
		public int ConnectingRoomId;

		public Door(World.World world, Vector2 position)
			: base(world, position)
		{
			Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 16);
			ConnectingRoomId = 0;
		}

		public override void OnEntityHit(Entity entity)
		{
			if (entity is PlayerBase)
			{
				PlayerBase player = entity as PlayerBase;
				player.Position = new Vector2(20, 20);

				World.LoadRoom(ConnectingRoomId);
			}
		}
	}
}
