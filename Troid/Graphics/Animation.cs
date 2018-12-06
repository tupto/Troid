using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Troid.Graphics
{
    public class Animation
    {
        public Rectangle[] Frames;
        public bool Loop;
        public int CurrFrame;
        public float FrameTime;
        public bool Finished;

        private float animationTimer;

        public Animation(Rectangle[] frames)
        {
            Frames = frames;
            FrameTime = 0.2f;
            Finished = false;
        }

        public Rectangle GetFrame()
        {
            return Frames[CurrFrame];
        }

        public void Update(GameTime gameTime)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer >= FrameTime)
            {
                animationTimer = 0;

                if (CurrFrame != Frames.Length - 1)
                {
                    CurrFrame++;
                }
                else if (Loop)
                {
                    CurrFrame = 0;
                }
                else
                {
                    Finished = true;
                }
            }
        }

        public void Reset()
        {
            CurrFrame = 0;
            animationTimer = 0;
        }
    }
}
