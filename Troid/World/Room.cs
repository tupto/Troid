using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troid.Entities;
using Troid.Physics;

namespace Troid.World
{
    public class Room
    {
        public int Width;
        public int Height;
        public Tile[,] Tiles;
        public List<Entity> Entities;

        private Quadtree quad;

        public Room(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            Entities = new List<Entity>();

            quad = new Quadtree(0, new Rectangle(0, 0, width * Tile.TILE_WIDTH, height * Tile.TILE_HEIGHT));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1 || (x > width / 2 && y > height / 2))
                    {
                        Tiles[x, y] = new Tile((x + y) % 2);
                    }
                }
            }
        }

        public bool TileHasCollision(int x, int y)
        {
            if (Tiles.Length <= x + y * Width || x + y * Width < 0 || x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            if (Tiles[(int)x, (int)y] != null)
            {
                return Tiles[(int)x, (int)y].Solid;
            }

            return false;
        }

        public Rectangle GetTileBouds(int x, int y)
        {
            return new Rectangle(x * Tile.TILE_WIDTH, y * Tile.TILE_HEIGHT, Tile.TILE_WIDTH, Tile.TILE_HEIGHT);
        }

        public void Update(GameTime gameTime)
        {
            quad.Clear();

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                quad.Insert(Entities[i]);
            }

            List<Entity> objects = new List<Entity>();
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities[i].Update(gameTime);

                objects.Clear();
                objects = quad.Retreive(objects, Entities[i]);

                for (int j = 0; j < objects.Count; j++)
                {
                    if (Entities[i] != objects[j] && Entities[i].Hitbox.Intersects(objects[j].Hitbox))
                    {
                        Entities[i].OnEntityHit(objects[j]);
                    }
                }

                if (!Entities[i].Alive)
                {
                    Entities[i].OnDeath();
                    Entities[i] = null;
                    Entities.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y] != null)
                        Tiles[x, y].Draw(x, y, spriteBatch);
                }
            }

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities[i].Draw(spriteBatch);
            }
        }
    }
}
