using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.World;

namespace Troid.Entities
{
    public class Player : Entity
    {
        private float shootTimer;
        private float shootTimerMax;
        private Vector2 shootOffset;

        public Player(World.World world)
            : base(world)
        {
            shootTimer = 0.0f;
            shootTimerMax = 0.3f;

            Animations.Add("idle", new Graphics.Animation(new Rectangle[] {
                new Rectangle(0, 0, 16, 16)
            }));

            Graphics.Animation walk = new Graphics.Animation(new Rectangle[] {
                new Rectangle(0, 16, 16, 16),
                new Rectangle(16, 16, 16, 16),
                new Rectangle(32, 16, 16, 16),
                new Rectangle(0, 32, 16, 16),
            });
            walk.Loop = true;
            walk.FrameTime = 0.2f;

            Animations.Add("walk", walk);

            Animations.Add("jump", new Graphics.Animation(new Rectangle[] {
                new Rectangle(48, 16, 16, 16)
            }));

            CurrAnimation = "idle";
            Position = new Vector2(140, 72);
            shootOffset = new Vector2(16, 8);
        }

        public override void Update(GameTime gameTime)
        {
            if (shootTimer < shootTimerMax)
            {
                shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.A))
            {
                Velocity.X = -MoveAcceleration * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                CurrAnimation = "walk";
                Direction = Direction.Left;
            }
            else if (ks.IsKeyDown(Keys.D))
            {
                Velocity.X = MoveAcceleration * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                CurrAnimation = "walk";
                Direction = Direction.Right;
            }
            else
            {
                Velocity.X = 0;
                CurrAnimation = "idle";
            }
            
            if (ks.IsKeyDown(Keys.LeftShift) && shootTimer >= shootTimerMax)
            {
                shootTimer = 0.0f;
                Vector2 shootPos = Position;
                if (Direction == Direction.Right)
                {
                    shootPos += new Vector2(16, 8);
                }
                else
                {
                    shootPos += new Vector2(0, 8);
                }

                Beam beam = new Beam(World, shootPos, Direction);
                World.CurrentRoom.AddEntity(beam);
            }

            Jumping = ks.IsKeyDown(Keys.Space);

            if (Jumping || !OnGround)
                CurrAnimation = "jump";

            base.Update(gameTime);
        }
    }
}
