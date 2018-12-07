using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.Entities;

namespace Troid.Physics
{
    public class Quadtree
    {
        private const int MAX_OBJECTS = 5;
        private const int MAX_LEVELS = 5;

        private int level;
        private List<Entity> objects;
        private Rectangle bounds;
        private Quadtree[] nodes;

        public Quadtree(int level, Rectangle bounds)
        {
            this.level = level;
            this.objects = new List<Entity>();
            this.bounds = bounds;
            nodes = new Quadtree[4];
        }

        public void Clear()
        {
            objects.Clear();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }

        public void Insert(Entity rect)
        {
            if (nodes[0] != null)
            {
                int index = GetIndex(rect);

                if (index != -1)
                {
                    nodes[index].Insert(rect);
                    return;
                }
            }

            objects.Add(rect);

            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(objects[i]);
                    if (index != -1)
                    {
                        nodes[index].Insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public List<Entity> Retreive(List<Entity> returnObjects, Entity rect)
        {
            int index = GetIndex(rect);
            if (index != -1 && nodes[0] != null)
            {
                returnObjects = nodes[index].Retreive(returnObjects, rect);
            }

            returnObjects.AddRange(objects);

            return returnObjects;
        }

        private void Split()
        {
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int x = bounds.X;
            int y = bounds.Y;

            nodes[0] = new Quadtree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[1] = new Quadtree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[2] = new Quadtree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new Quadtree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private int GetIndex(Entity rect)
        {
            int index = -1;
            float verticalMidpoint = bounds.X + ((float)bounds.Width / 2);
            float horizontalMidpoint = bounds.X + ((float)bounds.Width / 2);

            bool topQuadrant = rect.Hitbox.Y < horizontalMidpoint && rect.Hitbox.Y + rect.Hitbox.Height < horizontalMidpoint;
            bool bottomQuadrant = rect.Hitbox.Y > horizontalMidpoint;
            bool leftQuadrant = rect.Hitbox.X < verticalMidpoint && rect.Hitbox.X + rect.Hitbox.Width < verticalMidpoint;
            bool rightQuadrant = rect.Hitbox.X > verticalMidpoint;

            if (leftQuadrant)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            else if (rightQuadrant)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }
    }
}
