using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TroidEngine.Graphics.UI
{
	public class Button : UIComponent
	{
		public Button(string name, Rectangle bounds, string text = "")
			: base(name, bounds)
		{
			Bounds = bounds;
			Text = text;
			BorderWidth = 1;
			BorderColour = Color.Black;
			BodyColour = Color.Gray;
			HoverColour = Color.DarkGray;
			TextColour = Color.Black;

			OnClick += Button_OnClick;
		}

		private void Button_OnClick()
		{
			HasFocus = false;
		}
	}
}
