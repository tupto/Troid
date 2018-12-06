﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.World;

namespace Troid.Entities
{
    public class Beam : Entity
    {
        public static Texture2D BeamTex;

        public Beam(Room room, Vector2 position, Direction direction) : base(room)
        {
            Position = position;
            Direction = direction;
            MoveAcceleration = 200;
            if (direction == Direction.Right)
            {
                Velocity = new Vector2(MoveAcceleration, 0);
            }
            else
            {
                Velocity = new Vector2(-MoveAcceleration, 0);
            }

            ApplyGravity = false;

            if (BeamTex != null)
            {
                SpriteSheet = BeamTex;

                Animations.Add("move", new Graphics.Animation(new Rectangle[]
                {
                    new Rectangle(0, 0, 4, 4)
                }));

                Animations.Add("splode", new Graphics.Animation(new Rectangle[]
                {
                    new Rectangle(4, 0, 4, 4),
                    new Rectangle(8, 0, 4, 4),
                    new Rectangle(12, 0, 4, 4)
                }));
                Animations["splode"].FrameTime = 0.2f;

                CurrAnimation = "move";
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrAnimation == "splode")
            {
                if (Animations[CurrAnimation].Finished)
                {
                    Alive = false;
                }
            }

            base.Update(gameTime);
        }

        public override void OnWallHit()
        {
            if (CurrAnimation != "splode")
                CurrAnimation = "splode";
        }
    }
}