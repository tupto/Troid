using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TroidEngine.Graphics.UI
{
	public abstract class UIComponent
	{
		public static Texture2D Pixel;
		public static SpriteFont Font;

		public string Name;
		public Rectangle Bounds;
		public string Text;
		public bool HasFocus;
		public int BorderWidth;
		public Color BorderColour;
		public Color BodyColour;
		public Color HoverColour;
		public Color TextColour;

		private bool mousedIn;

		public delegate void OnClickEventHandler();
		public event OnClickEventHandler OnClick;

		public delegate void OnMouseInEventHandler();
		public event OnMouseInEventHandler OnMouseIn;

		public delegate void OnMouseOutEventHandler();
		public event OnMouseOutEventHandler OnMouseOut;

		public UIComponent(string name, Rectangle bounds)
		{
			this.Name = name;
			this.Bounds = bounds;
			this.BodyColour = Color.Gray;
			this.BorderColour = Color.Black;
			this.HoverColour = Color.Magenta;
			this.TextColour = Color.Black;
			this.BorderWidth = 1;

			OnClick += NoOp;
			OnMouseIn += NoOp;
			OnMouseOut += NoOp;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (BorderWidth != 0)
			{
				spriteBatch.Draw(Pixel, Bounds, BorderColour);
			}

			Rectangle bodyBounds = GetBodyBounds();

			if (!mousedIn || (mousedIn && HoverColour == Color.Magenta))
			{
				if (BodyColour != Color.Magenta)
				{
					spriteBatch.Draw(Pixel, bodyBounds, BodyColour);
				}
			}
			else
			{
				spriteBatch.Draw(Pixel, bodyBounds, HoverColour);
			}

			if (Text != "")
			{
				string textToDraw = FormatText();
				Vector2 textSize = Font.MeasureString(textToDraw);
				Vector2 textDrawPoint = new Vector2(
					Bounds.Center.X - (textSize.X / 2),
					Bounds.Center.Y - (textSize.Y / 2)
				);
				spriteBatch.DrawString(Font, textToDraw, textDrawPoint, TextColour);
			}
		}

		protected virtual Rectangle GetBodyBounds()
		{
			Rectangle bodyBounds = Bounds;
			bodyBounds.Inflate(-BorderWidth * 2, -BorderWidth * 2);
			return bodyBounds;
		}

		protected string FormatText()
		{
			return CropText(Text);
		}

		protected string CropText(string text)
		{
			Vector2 textSize = Font.MeasureString(text);
			if (textSize.X > Bounds.Width - (BorderWidth * 2) - 4)
			{
				if (text.Length == 1)
					return "";
				return CropText(text.Substring(1));
			}

			return text;
		}

		public void Click()
		{
			OnClick();
		}

		public void MouseIn()
		{
			if (!mousedIn)
			{
				mousedIn = true;
				OnMouseIn();
			}
		}

		public void MouseOut()
		{
			if (mousedIn)
			{
				mousedIn = false;
				OnMouseOut();
			}
		}

		protected void NoOp() { }
	}
}
