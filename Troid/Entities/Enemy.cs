using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.World;

namespace Troid.Entities
{
    public class Enemy : Entity
    {
        public static Texture2D EnemySprite;

        public int ContactDamage;

        public Enemy(World.World world) : base(world)
        {
            Position = new Vector2(16, 24);

            Graphics.Animation spin = new Graphics.Animation(new Rectangle[] {
                new Rectangle(0, 0, 16, 16),
                new Rectangle(16, 0, 16, 16),
                new Rectangle(32, 0, 16, 16),
                new Rectangle(48, 0, 16, 16),
                new Rectangle(64, 0, 16, 16),
                new Rectangle(80, 0, 16, 16),
                new Rectangle(96, 0, 16, 16),
                new Rectangle(112, 0, 16, 16)
            });
            spin.Loop = true;
            spin.FrameTime = 0.2f;

            Animations.Add("spin", spin);

            CurrAnimation = "spin";

            ContactDamage = 20;
        }

        public override void OnEntityHit(Entity entity)
        {
            if (entity is Player)
            {
                entity.AdjustHealth(-ContactDamage);
                entity.Knockback(new Vector2(entity.Hitbox.Center.X - Hitbox.Center.X, entity.Hitbox.Center.Y - Hitbox.Center.Y));
            }
        }
    }
}
