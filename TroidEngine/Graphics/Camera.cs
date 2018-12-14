using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.Entities;
using TroidEngine.World;

namespace TroidEngine.Graphics
{
	public class Camera
	{
		public float Zoom;
		public Vector2 Position;

		private World.World world;
		private Rectangle screenBounds;

		public Camera(World.World world, Viewport viewport)
		{
			this.world = world;
			this.screenBounds = viewport.Bounds;
			Position = Vector2.Zero;
			Zoom = 2.0f;
		}

		public Vector2 GetGameLocation(Vector2 location)
		{
			Vector2 pixelLocation = Vector2.Transform(location,
			                                          Matrix.Invert(GetTransform()));

			return pixelLocation;
		}

		public Matrix GetTransform()
		{
			PlayerBase player = world.CurrentRoom.GetPlayer();

			if (player != null)
			{
				Position = player.Position;
			}

			Position.X = MathHelper.Clamp(Position.X, screenBounds.Width / Zoom * 0.5f, world.CurrentRoom.PixelWidth - (screenBounds.Width / Zoom * 0.5f));
			Position.Y = MathHelper.Clamp(Position.Y, screenBounds.Height / Zoom * 0.5f, world.CurrentRoom.PixelHeight - (screenBounds.Height / Zoom * 0.5f));

			return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0.0f)) *
					Matrix.CreateScale(Zoom) *
					Matrix.CreateTranslation(new Vector3(screenBounds.Width * 0.5f, screenBounds.Height * 0.5f, 0));
		}
	}
}
