using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TroidEngine.World
{
	public class Tile
	{
		public const int TILE_HEIGHT = 8;
		public const int TILE_WIDTH = 8;

		private static Rectangle[] sourceRects;
		private static Texture2D tileSheet;
		public static Texture2D TileSheet
		{
			get { return tileSheet; }
			set
			{
				tileSheet = value;

				int numWide = tileSheet.Width / TILE_WIDTH;
				int numHigh = tileSheet.Height / TILE_HEIGHT;
				sourceRects = new Rectangle[numWide * numHigh];

				for (int y = 0; y < numHigh; y++)
				{
					for (int x = 0; x < numWide; x++)
					{
						sourceRects[x + numWide * y] = new Rectangle(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT);
					}
				}
			}
		}

		public static int NumTiles
		{
			get { return sourceRects?.Length ?? 0; }
		}

		public int ID;
		public TileCollision CollisionType;

		public Tile(int id, TileCollision collisionType = TileCollision.Solid)
		{
			ID = id;
			CollisionType = collisionType;
		}

		public void Draw(int x, int y, SpriteBatch spriteBatch)
		{
			Color colour = Color.White;
			if (CollisionType == TileCollision.Water)
			{
				colour = new Color(255, 255, 255, 100);
			}

			spriteBatch.Draw(tileSheet, new Vector2(x * TILE_WIDTH, y * TILE_HEIGHT),
							 sourceRects[ID], colour, 0, Vector2.Zero, new Vector2(1, 1),
							 SpriteEffects.None, 0.0f);
		}
	}

	public enum TileCollision
	{
		None,
		Solid,
		Water
	}
}
