using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TroidEngine.Graphics.UI
{
	public class Icon : UIComponent
	{
		public Texture2D IconSheet;
		public Rectangle SourceRectangle;

		public Icon(string name, Rectangle bounds, Texture2D iconSheet, Rectangle? sourceRect)
			: base(name, bounds)
		{
			Bounds = bounds;
			IconSheet = iconSheet;
			SourceRectangle = sourceRect ?? new Rectangle(0, 0, Bounds.Width, Bounds.Height);
			BorderWidth = 1;
			BorderColour = Color.Black;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Pixel, Bounds, BorderColour);
			spriteBatch.Draw(IconSheet, new Vector2(Bounds.X + BorderWidth, Bounds.Y + BorderWidth), SourceRectangle, Color.White);
		}
	}
}
