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

        public Enemy(Room room) : base(room)
        {
            Position = new Vector2(16, 24);

            Graphics.Animation spin = new Graphics.Animation(new Rectangle[] {
                new Rectangle(0, 0, 16, 16),
                new Rectangle(16, 0, 16, 16),
                new Rectangle(32, 0, 16, 16),
                new Rectangle(48, 0, 16, 16)
            });
            spin.Loop = true;
            spin.FrameTime = 0.2f;

            Animations.Add("float", spin);

            CurrAnimation = "float";

            ContactDamage = 0;

			ApplyGravity = false;
        }

		public override void Update(GameTime gameTime)
		{
			Player player = Room.GetPlayer();

			if (player != null)
			{
				Vector2 playerDir = player.Position - Position;
				playerDir.Normalize();

				Velocity = playerDir * MoveAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (Velocity.X > 0)
				{
					Direction = Direction.Right;
				}
				else
				{
					Direction = Direction.Left;
				}
			}
			else
			{
				Velocity = Vector2.Zero;
			}

			base.Update(gameTime);
		}

        public override void OnEntityHit(Entity entity)
        {
            if (entity is Player)
            {
                entity.AdjustHealth(-ContactDamage);
                entity.Knockback(new Vector2(entity.Hitbox.Center.X - Hitbox.Center.X, entity.Hitbox.Center.Y - Hitbox.Center.Y));
            }
        }

		public override void OnDeath()
		{
			Enemy newE = new Enemy(Room);
			newE.SpriteSheet = this.SpriteSheet;
			Room.Entities.Add(newE);
		}
    }
}
