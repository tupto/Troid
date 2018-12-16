using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TroidEngine.Graphics.UI
{
	public class TextBox : UIComponent
	{
		private float keyTimer = 0.0f;
		private float allowAnotherPressTime = 0.1f;
		private Keys[] lastKeys;

		public TextBox(string name, Rectangle bounds)
			: base(name, bounds)
		{
			Bounds = bounds;
			Text = "";
			BorderWidth = 1;
			BorderColour = Color.Black;
			BodyColour = Color.White;
			TextColour = Color.Black;
			lastKeys = new Keys[0];
		}
	}
}
