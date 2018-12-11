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

		private int playerIndex = 0;
        public Quadtree quad;

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
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1 || y > height / 2)
                    {
						Tiles[x, y] = new Tile(0);
                    }
                }
            }

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (Tiles[x, y] == null)
						continue;

					if (x == 0)
					{
						if (y == 0 || y == height - 1)
						{
							Tiles[x, y] = new Tile(5);
						}
						else
						{
							Tiles[x, y] = new Tile(4);
						}
					}
					else if (y == 0)
					{
						Tiles[x, y] = new Tile(2);
					}
					else if (x == width - 1 && !(x > width / 2 && y > height / 2))
					{
						Tiles[x, y] = new Tile(3);
					}
					else if (y == height - 1 && !(x > width / 2 && y > height / 2))
					{
						Tiles[x, y] = new Tile(2);
					}
					else if (x - 1 == width / 2)
					{
						if (y - 1 == height / 2)
						{
							Tiles[x, y] = new Tile(0);
						}
						else
						{
							Tiles[x, y] = new Tile(3);
						}
					}
					else if (y - 1 == height / 2 && x - 1 > width / 2)
					{
						Tiles[x, y] = new Tile(2);
					}
					else if (y > height / 2 && x -1 < width / 2)
					{
						Tiles[x, y] = new Tile(8, TileCollision.Water);
					}
					else
					{
						Tiles[x, y] = new Tile(5);
					}
				}
			}
        }

		public Player GetPlayer()
		{
			if (Entities[playerIndex] is Player)
				return (Player)Entities[playerIndex];

			return null;
		}

        public TileCollision GetTileCollision(int x, int y)
        {
            if (Tiles.Length <= x + y * Width || x + y * Width < 0 || x < 0 || y < 0 || x >= Width || y >= Height)
				return TileCollision.None;

            if (Tiles[(int)x, (int)y] != null)
            {
				return Tiles[(int)x, (int)y].CollisionType;
            }

			return TileCollision.None;
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
			List<Tile> waterTiles = new List<Tile>();
			List<int> waterTilesX = new List<int>();
			List<int> waterTilesY = new List<int>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
					if (Tiles[x, y] != null)
					{
						if (Tiles[x, y].CollisionType != TileCollision.Water)
						{
							Tiles[x, y].Draw(x, y, spriteBatch);
						}
						else
						{
							waterTiles.Add(Tiles[x, y]);
							waterTilesX.Add(x);
							waterTilesY.Add(y);
						}
					}
                }
            }

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities[i].Draw(spriteBatch);
            }

			for (int i = 0; i < waterTiles.Count; i++)
			{
				waterTiles[i].Draw(waterTilesX[i], waterTilesY[i], spriteBatch);
			}
        }
    }
}
