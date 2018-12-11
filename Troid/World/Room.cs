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

        public Quadtree quad;

        public int PixelWidth
        {
            get { return Width * Tile.TILE_WIDTH; }
        }

        public int PixelHeight
        {
            get { return Height * Tile.TILE_HEIGHT; }
        }

        private List<Entity> entities;
        private int playerIndex;
        private World world;

        public Room(World world, int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            entities = new List<Entity>();
            playerIndex = -1;

            quad = new Quadtree(0, new Rectangle(0, 0, width * Tile.TILE_WIDTH, height * Tile.TILE_HEIGHT));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1 || (x > width / 4 && y > height / 4))
                    {
                        if (x == 0 && (y == height - 2 || y == height - 3))
                            continue;

                        Tiles[x, y] = new Tile((x + y) % 2);
                    }
                }
            }

            Door door = new Door(world, new Vector2(0, (height - 3) * Tile.TILE_HEIGHT));
            door.ConnectingRoomId = (width / 30) - 1;
            AddEntity(door);
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);

            if (entity is Player)
                playerIndex = entities.Count - 1;
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        public List<Entity> GetEntities()
        {
            return entities;
        }

        public Player GetPlayer()
        {
            if (playerIndex != -1)
            {
                return (Player)entities[playerIndex];
            }

            return null;
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

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                quad.Insert(entities[i]);
            }

            List<Entity> objects = new List<Entity>();
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Update(gameTime);

                objects.Clear();
                objects = quad.Retreive(objects, entities[i]);

                for (int j = 0; j < objects.Count; j++)
                {
                    if (entities[i] != objects[j] && entities[i].Hitbox.Intersects(objects[j].Hitbox))
                    {
                        entities[i].OnEntityHit(objects[j]);
                    }
                }

                if (!entities[i].Alive)
                {
                    if (entities[i] is Player)
                        playerIndex = -1;

                    entities[i].OnDeath();
                    entities[i] = null;
                    entities.RemoveAt(i);
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

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Draw(spriteBatch);
            }
        }
    }
}
