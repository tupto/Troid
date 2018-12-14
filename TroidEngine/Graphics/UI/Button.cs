using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TroidEngine.Grapihcs.UI
{
	public class Button
	{
		public Rectangle Bounds;
		public string Text;
		public int BorderWidth;
		public Color BorderColour;
		public Color BodyColour;
		public Color HoverColour;
		public Color TextColour;

		public static Texture2D Pixel;
		public static SpriteFont Font;

		public delegate void OnClickEventHandler();
		public event OnClickEventHandler OnClick;

		public delegate void OnMouseInEventHandler();
		public event OnMouseInEventHandler OnMouseIn;

		public delegate void OnMouseOutEventHandler();
		public event OnMouseOutEventHandler OnMouseOut;

		public Button(Rectangle bounds, string text = "")
		{
			Bounds = bounds;
			Text = text;
			BorderWidth = 1;
			BorderColour = Color.Black;
			BodyColour = Color.Gray;
			HoverColour = Color.DarkGray;
			TextColour = Color.Black;

			OnClick += NoOp;
			OnMouseIn += NoOp;
			OnMouseOut += NoOp;
		}

		public void Click()
		{
			OnClick();
		}

		public void MouseIn()
		{
			OnMouseIn();
		}

		public void MouseOut()
		{
			OnMouseOut();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Pixel, Bounds, BorderColour);

			Rectangle bodyBounds = Bounds;
			bodyBounds.Inflate(-BorderWidth * 2, -BorderWidth * 2);
			spriteBatch.Draw(Pixel, bodyBounds, BodyColour);

			Vector2 textSize = Font.MeasureString(Text);
			Vector2 textDrawPoint = new Vector2(
				Bounds.Center.X - (textSize.X / 2),
				Bounds.Center.Y - (textSize.Y / 2)
			);
			spriteBatch.DrawString(Font, Text, textDrawPoint, TextColour);
		}

		private void NoOp() { }
	}
}
