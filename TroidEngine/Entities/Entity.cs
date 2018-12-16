using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.Graphics;
using TroidEngine.World;
using TroidEngine.Physics;

namespace TroidEngine.Entities
{
	public abstract class Entity
	{
		public string Name;
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
		public World.World World;
		public Direction Direction;
		public bool Alive;

		public int Health = 100;
		public int MaxHealth = 100;
		public float MoveAcceleration = 4000.0f;
		public float GravityAcceleration = 800.0f;
		public float JumpLaunchVelocity = -900.0f;
		public float JumpControlPower = 0.2f;
		public float KnockbackVelocity = 300.0f;
		public float KnockbackControlPower = 0.2f;
		public float WaterSpeedModifier = 0.3f;
		public float MaxJumpTime = 0.50f;
		public float MaxKnockbackTime = 0.50f;
		public float KnockbackFlashInterval = 0.1f;
		public float DrownTime = 5.0f;
		public float MaxYSpeed = 200.0f;
		public float MaxXSpeed = 1000.0f;
		public float ShootTimer = 0.0f;
		public float ShootTimerMax = 0.3f;
		public Vector2 ShootOffset;

		public Color Colour;
		public Color DamageColour;

		private int previousBottom;

		protected bool wasJumping;
		protected float jumpTimer;
		protected bool hitSomething;
		protected bool wasInWater;
		protected float waterTimer;
		protected bool beingKnockedback;
		protected float knockbackTimer;
		protected float knockbackFlashTimer;
		protected Vector2 knockbackDirection;
		protected bool somethingBelow;

		public Entity(string name)
		{
			Name = name;
			Animations = new Dictionary<string, Animation>();
			Position = Vector2.Zero;
			Velocity = Vector2.Zero;
			ApplyGravity = true;
			Alive = true;
			Direction = Direction.Right;
			Colour = Color.White;
			DamageColour = new Color(255, 0, 0, 32);
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

			hitSomething = false;

			for (int x = worldLeft; x <= worldRight; x++)
			{
				for (int y = worldTop; y <= worldBottom; y++)
				{
					TileCollision colType = World.CurrentRoom.GetTileCollision(x, y);

					if (colType == TileCollision.Solid)
					{
						Rectangle tileBounds = World.CurrentRoom.GetTileBounds(x, y);
						hitSomething = PushOutOfTile(tileBounds) || hitSomething;

						if (y == worldBottom || y == worldBottom - 1)
						{
							somethingBelow = true;
						}
					}
					else if (colType == TileCollision.Water)
					{
						InWater = InWater || Hitbox.Intersects(World.CurrentRoom.GetTileBounds(x, y));
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
			if (!beingKnockedback)
			{
				if (direction == Vector2.Zero)
					direction = new Vector2(0, -1);

				knockbackDirection = direction;
				knockbackDirection.Normalize();
				knockbackTimer = 0.0f;
				beingKnockedback = true;
			}
		}

		private void DoKnockback(GameTime gameTime)
		{
			if (beingKnockedback)
			{
				knockbackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
				knockbackFlashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (knockbackTimer <= MaxKnockbackTime)
				{
					Velocity += KnockbackVelocity * knockbackDirection * (1.0f - (float)Math.Pow(knockbackTimer / MaxKnockbackTime, KnockbackControlPower));
				}
				else
				{
					beingKnockedback = false;
				}

				if (knockbackFlashTimer <= KnockbackFlashInterval)
				{
					Colour = Colour == Color.White ? DamageColour : Color.White;
					knockbackFlashTimer = 0.0f;
				}
			}
			else if ( Colour == DamageColour)
			{
				Colour = Color.White;
			}
		}

		private void DoJump(GameTime gameTime)
		{
			if (Jumping)
			{
				if ((!wasJumping && (OnGround || InWater || Velocity.Y == MaxYSpeed)) || jumpTimer > 0.0f)
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
			}

			DoJump(gameTime);
			DoKnockback(gameTime);
			DoWater(gameTime);

			if (Velocity.Y > MaxYSpeed)
				Velocity.Y = MaxYSpeed;
			else if (Velocity.Y < -MaxYSpeed)
				Velocity.Y = -MaxYSpeed;
			else if (float.IsNaN(Velocity.Y))
				Velocity.Y = 0;

			if (Velocity.X > MaxXSpeed)
				Velocity.X = MaxXSpeed;
			else if (Velocity.X < -MaxXSpeed)
				Velocity.X = -MaxXSpeed;
			else if (float.IsNaN(Velocity.X))
				Velocity.X = 0;

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
							 Colour, 0, Vector2.Zero, new Vector2(1, 1), effects, 0.0f);
		}

		public virtual void OnWallHit()
		{
			beingKnockedback = false;
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
