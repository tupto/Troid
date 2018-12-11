using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Troid.World;

namespace Troid.Entities
{
    public abstract class TriggerEntity : Entity
    {
        public TriggerEntity(World.World world, Vector2 position) : base(world)
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
