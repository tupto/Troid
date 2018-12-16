using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TroidEngine.World;

namespace TroidEngine.Entities
{
	public abstract class TriggerEntity : Entity
	{
		public TriggerEntity(string name, Vector2 position) : base(name)
		{
			Position = position;
		}

		public override void Update(GameTime gameTime)
		{
			return;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			return;
		}
	}
}
