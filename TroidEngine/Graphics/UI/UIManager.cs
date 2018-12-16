using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TroidEngine.Graphics.UI
{
	public class UIManager
	{
		public int Width;
		public int Height;

		public bool UIClicked;
		public bool UIInUse;

		private Dictionary<string, UIComponent> components;

		public UIManager(Game game)
		{
			components = new Dictionary<string, UIComponent>();
			Width = game.GraphicsDevice.Viewport.Width;
			Height = game.GraphicsDevice.Viewport.Height;

			game.Window.TextInput += (sender, e) =>
			{
				foreach (UIComponent uic in components.Values)
				{
					if (e.Key == Keys.Escape)
						uic.HasFocus = false;

					if ((uic) is TextBox)
					{
						TextBox tb = (uic) as TextBox;
						if (tb.HasFocus)
						{
							if (e.Key == Keys.Back)
							{
								if (tb.Text.Length != 0)
									tb.Text = tb.Text.Remove(tb.Text.Length - 1);
							}
							else
							{
								if (e.Key == Keys.Enter)
									return;
								tb.Text += e.Character;
							}
						}
					}
				}
			};
		}

		public void AddComponent(UIComponent component)
		{
			components.Add(component.Name, component);
		}

		public UIComponent GetComponent(string name)
		{
			if (components.ContainsKey(name))
				return components[name];
			else
				throw new ArgumentOutOfRangeException("This object contains no UIComponent named {0}", name);
		}

		public void Update(GameTime gameTime)
		{
			UIInUse = false;
			UIClicked = false;
			MouseState ms = Mouse.GetState();

			foreach (UIComponent uic in components.Values)
			{
				if (uic.HasFocus)
				{
					UIInUse = true;
				}

				if (uic.Bounds.Contains(ms.X, ms.Y))
				{
					if (ms.LeftButton == ButtonState.Pressed)
					{
						uic.Click();
						uic.HasFocus = true;
						UIClicked = true;
					}
					else
					{
						uic.MouseIn();
					}
				}
				else
				{
					if (uic.HasFocus && ms.LeftButton == ButtonState.Pressed)
					{
						uic.HasFocus = false;
					}
					uic.MouseOut();
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (UIComponent uic in components.Values)
			{
				uic.Draw(spriteBatch);
			}
		}
	}
}
