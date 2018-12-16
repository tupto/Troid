using System;
using Microsoft.Xna.Framework;

namespace TroidEngine.Graphics.UI
{
	public class Label : UIComponent
	{
		public Label(string name, Rectangle bounds, string text)
			: base(name, bounds)
		{
			BodyColour = Color.Magenta;
			BorderWidth = 0;
			Text = text;

			OnClick += Label_OnClick;
		}

		void Label_OnClick()
		{
			HasFocus = false;
		}
	}
}
