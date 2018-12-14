using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.World;
using TroidEngine.Entities;
using TroidEngine.Graphics;

namespace Troid.Entities
{
    public class Enemy : Entity
    {
        public static Texture2D EnemySprite;

        public int ContactDamage;

        public Enemy(World world) : base(world)
        {
            Position = new Vector2(16, 24);

            Animation spin = new Animation(new Rectangle[] {
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
			MoveAcceleration = 1000.0f;

			ApplyGravity = false;
        }

		public override void Update(GameTime gameTime)
		{
			Player player = (Player)World.CurrentRoom.GetPlayer();

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
			Enemy newE = new Enemy(World);
			newE.SpriteSheet = this.SpriteSheet;
			World.CurrentRoom.AddEntity(newE);
		}
    }
}
