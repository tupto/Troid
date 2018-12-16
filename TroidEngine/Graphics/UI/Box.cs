using System;
using Microsoft.Xna.Framework;

namespace TroidEngine.Graphics.UI
{
	public class Box : UIComponent
	{
		public Box(string name, Rectangle bounds)
			: base (name, bounds)
		{
			BorderWidth = 0;
		}
	}
}
