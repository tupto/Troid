using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.Graphics;
using Troid.Utils;
using Troid.World;

namespace Troid.Entities
{
    public abstract class Entity
    {
        public Dictionary<string, Animation> Animations;
        public string CurrAnimation;
        public Texture2D SpriteSheet;
        public Vector2 Position;
        public Vector2 Velocity;
        public Rectangle Hitbox;
        public bool Jumping;
        public bool ApplyGravity;
        public bool OnGround;
		public bool InWater;
        public Room Room;
        public Direction Direction;
        public bool Alive;

        public int Health = 100;
        public int MaxHealth = 100;
        public float MoveAcceleration = 2000.0f;
        public float GravityAcceleration = 800.0f;
        public float JumpLaunchVelocity = -900.0f;
        public float JumpControlPower = 0.2f;
        public float KnockbackVelocity = 500.0f;
        public float KnockbackControlPower = 0.2f;
		public float WaterSpeedModifier = 0.3f;
        public float MaxJumpTime = 0.50f;
        public float MaxKnockbackTime = 0.50f;
		public float DrownTime = 5.0f;
        public float MaxYSpeed = 500.0f;
        public float MaxXSpeed = 500.0f;

        private int previousBottom;

        private bool wasJumping;
        private float jumpTimer;
		private bool wasInWater;
		private float waterTimer;
        private bool beingKnockedback;
        private float knockbackTimer;
        private Vector2 knockbackDirection;
        private bool somethingBelow;

        public Entity(Room room)
        {
            Room = room;
            Animations = new Dictionary<string, Animation>();
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            ApplyGravity = true;
            Alive = true;
            Direction = Direction.Right;
        }

        protected void UpdateHitbox()
        {
            if (Animations.ContainsKey(CurrAnimation))
            {
                Rectangle frame = Animations[CurrAnimation].GetFrame();
                Hitbox = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), frame.Width, frame.Height);
            }
            else
            {
                Hitbox = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), 1, 1);
            }
        }

        public virtual void HandleRoomCollisions()
        {
            UpdateHitbox();

            int worldLeft = (int)Math.Floor((float)Hitbox.Left / Tile.TILE_WIDTH);
            int worldRight = (int)Math.Ceiling((float)Hitbox.Right / Tile.TILE_WIDTH) - 1;
            int worldTop = (int)Math.Floor((float)Hitbox.Top / Tile.TILE_HEIGHT);
            int worldBottom = (int)Math.Ceiling((float)Hitbox.Bottom / Tile.TILE_HEIGHT);

			InWater = false;
            OnGround = previousBottom == Hitbox.Bottom && Velocity.Y == 0;
            somethingBelow = false;

            bool hitSomething = false;

            for (int x = worldLeft; x <= worldRight; x++)
            {
                for (int y = worldTop; y <= worldBottom; y++)
                {
					TileCollision colType = Room.GetTileCollision(x, y);

					if (colType == TileCollision.Solid)
					{
						Rectangle tileBounds = Room.GetTileBouds(x, y);
						hitSomething = PushOutOfTile(tileBounds) || hitSomething;

						if (y == worldBottom || y == worldBottom - 1)
						{
							somethingBelow = true;
						}
					}
					else if (colType == TileCollision.Water)
					{
						InWater = InWater || Hitbox.Intersects(Room.GetTileBouds(x, y));
					}
                }
            }

            if (!somethingBelow)
                OnGround = false;

            previousBottom = Hitbox.Bottom;

            if (hitSomething)
                OnWallHit();
        }

        public void AdjustHealth(int amount)
        {
            Health += amount;

            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
            else if (Health <= 0)
            {
                Health = 0;
                Alive = false;
            }
        }

        public void Knockback(Vector2 direction)
        {
            knockbackDirection = direction;
            knockbackDirection.Normalize();
            knockbackTimer = 0.0f;
            beingKnockedback = true;
        }

        private void DoKnockback(GameTime gameTime)
        {
            if (beingKnockedback)
            {
                knockbackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (knockbackTimer <= MaxKnockbackTime)
                {
                    Velocity = KnockbackVelocity * knockbackDirection * (1.0f - (float)Math.Pow(knockbackTimer / MaxKnockbackTime, KnockbackControlPower));
                }
                else
                {
                    beingKnockedback = false;
                }
            }
        }

        private void DoJump(GameTime gameTime)
        {
            if (Jumping)
            {
				if ((!wasJumping && (OnGround || InWater)) || jumpTimer > 0.0f)
                {
                    jumpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (jumpTimer > 0.0f && jumpTimer <= MaxJumpTime)
                {
                    Velocity.Y = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTimer / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    jumpTimer = 0.0f;
                }
            }
            else
            {
                jumpTimer = 0.0f;
            }

            wasJumping = Jumping;
        }

		public void DoWater(GameTime gameTime)
		{
			if (InWater)
			{
				waterTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (waterTimer >= DrownTime)
				{
					AdjustHealth(-25);
					Knockback(new Vector2(0, -1));
					waterTimer = 0.2f * DrownTime;
				}

				Velocity *= WaterSpeedModifier;
			}
			else
			{
				waterTimer = 0.0f;
			}

			wasInWater = InWater;
		}

        private bool PushOutOfTile(Rectangle tileBounds)
        {
            Vector2 collisionDept = Hitbox.GetIntersectionDepth(tileBounds);

            if (collisionDept != Vector2.Zero)
            {
                float absDepthX = Math.Abs(collisionDept.X);
                float absDepthY = Math.Abs(collisionDept.Y);

                if (absDepthY < absDepthX)
                {
                    OnGround = previousBottom <= tileBounds.Top;

                    if (OnGround)
                    {
                        Position = new Vector2(Position.X, tileBounds.Top - Hitbox.Height);
                        UpdateHitbox();
                    }
                    else
                    {
                        Position = new Vector2(Position.X, (float)Math.Round(Position.Y) + collisionDept.Y + 1);
                        wasJumping = false;
                        jumpTimer = MaxJumpTime;
                        Velocity.Y = 0;
                        UpdateHitbox();
                    }

                    PushOutOfTile(tileBounds);
                }
                else
                {
                    Position = new Vector2(Position.X + collisionDept.X, Position.Y);
                    UpdateHitbox();
                }
                return true;
            }
            return false;
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)(gameTime.ElapsedGameTime.TotalSeconds);
            Vector2 previousPosition = Position;

            if (Animations.ContainsKey(CurrAnimation))
            {
                Animations[CurrAnimation].Update(gameTime);
                Rectangle frame = Animations[CurrAnimation].GetFrame();
            }

            if (ApplyGravity && !OnGround)
            {
                Velocity.Y += GravityAcceleration * elapsed;

                if (Velocity.Y > MaxYSpeed)
                    Velocity.Y = MaxYSpeed;
            }

            DoJump(gameTime);
            DoKnockback(gameTime);
			DoWater(gameTime);

            Position += Velocity * elapsed;

            HandleRoomCollisions();

			wasInWater = InWater;

            if (Position.X == previousPosition.X)
                Velocity.X = 0;
            if (Position.Y == previousPosition.Y)
                Velocity.Y = 0;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects effects = SpriteEffects.None;

            if (Direction == Direction.Left)
                effects = SpriteEffects.FlipHorizontally;

            Rectangle? animationRect = null;
            if (Animations.ContainsKey(CurrAnimation))
            {
                animationRect = Animations[CurrAnimation].GetFrame();
            }

			spriteBatch.Draw(SpriteSheet, Position, animationRect,
			                 Color.White, 0, Vector2.Zero, new Vector2(1, 1), effects, 0.0f);
        }

        public virtual void OnWallHit()
		{
		}
        public virtual void OnEntityHit(Entity entity) { }
        public virtual void OnDeath() { }
    }

    public enum Direction
    {
        Left,
        Right
    }
}
