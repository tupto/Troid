using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TroidEngine.Entities;
using TroidEngine.Physics;
using System.Reflection;

namespace TroidEngine.World
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

		public Room(int width, int height)
		{
			Width = width;
			Height = height;
			Tiles = new Tile[width, height];
			entities = new List<Entity>();
			playerIndex = -1;

			quad = new Quadtree(0, new Rectangle(0, 0, width * Tile.TILE_WIDTH, height * Tile.TILE_HEIGHT));

			Door door = new Door(world, new Vector2(0, (height - 3) * Tile.TILE_HEIGHT));
			door.ConnectingRoomId = (width / 30) - 1;
			AddEntity(door);
		}

		public int GetTileID(int x, int y)
		{
			if (Tiles[x, y] != null)
				return Tiles[x, y].ID;
			return -1;
		}

		public void AddEntity(Entity entity)
		{
			entities.Add(entity);

			if (entity is PlayerBase)
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

		public PlayerBase GetPlayer()
		{
			if (playerIndex != -1 && entities[playerIndex] is PlayerBase)
				return (PlayerBase)entities[playerIndex];

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

		public Rectangle GetTileBounds(int x, int y)
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
					if (typeof(PlayerBase).GetTypeInfo().IsAssignableFrom(entities[i].GetType().GetTypeInfo()))
					{
						playerIndex = -1;
					}

					entities[i].OnDeath();
					entities[i] = null;
					entities.RemoveAt(i);
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

			for (int i = entities.Count - 1; i >= 0; i--)
			{
				entities[i].Draw(spriteBatch);
			}

			for (int i = 0; i < waterTiles.Count; i++)
			{
				waterTiles[i].Draw(waterTilesX[i], waterTilesY[i], spriteBatch);
			}
		}

		public void DrawCollisionBoxes(SpriteBatch spriteBatch, Action<Rectangle, int, Color> drawBorder)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (Tiles[x, y] != null)
					{
						Rectangle collisionBounds = GetTileBounds(x, y);
						Color collisionColour = Color.White;

						switch (Tiles[x, y].CollisionType)
						{
							case TileCollision.None:
								continue;
							case TileCollision.Solid:
								collisionColour = Color.Red;
								break;
							case TileCollision.Water:
								collisionColour = Color.Blue;
								break;
						}

						drawBorder(collisionBounds, 1, collisionColour);
					}
				}
			}
		}
	}
}
