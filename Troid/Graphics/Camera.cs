using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.Entities;
using Troid.World;

namespace Troid.Graphics
{
    public class Camera
    {
        public float Zoom;

        private World.World world;
        private Vector2 position;
        private Rectangle screenBounds;

        public Camera(World.World world, Viewport viewport)
        {
            this.world = world;
            this.screenBounds = viewport.Bounds;
            position = Vector2.Zero;
            Zoom = 2.0f;
        }

        public Matrix GetTransform()
        {
            Player player = world.CurrentRoom.GetPlayer();

            if (player != null)
            {
                position = player.Position;
                position.X = MathHelper.Clamp(position.X, screenBounds.Width / Zoom * 0.5f, world.CurrentRoom.PixelWidth - (screenBounds.Width / Zoom * 0.5f));
                position.Y = MathHelper.Clamp(position.Y, screenBounds.Height / Zoom * 0.5f, world.CurrentRoom.PixelHeight - (screenBounds.Height / Zoom * 0.5f));
            }

            return Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0.0f)) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new Vector3(screenBounds.Width * 0.5f, screenBounds.Height * 0.5f, 0));
        }
    }
}
