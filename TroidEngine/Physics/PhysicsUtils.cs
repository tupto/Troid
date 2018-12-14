using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TroidEngine.Physics
{
	public static class PhysicsUtils
	{
		public static Vector2 GetIntersectionDepth(this Rectangle a, Rectangle b)
		{
			if (!a.Intersects(b))
				return Vector2.Zero;

			float halfWidthA = a.Width / 2.0f;
			float halfHeightA = a.Height / 2.0f;
			float halfWidthB = b.Width / 2.0f;
			float halfHeightB = b.Height / 2.0f;

			Vector2 centerA = new Vector2(a.Left + halfWidthA, a.Top + halfHeightA);
			Vector2 centerB = new Vector2(b.Left + halfWidthB, b.Top + halfHeightB);

			float xDist = centerA.X - centerB.X;
			float yDist = centerA.Y - centerB.Y;

			float minXDist = halfWidthA + halfWidthB;
			float minYDist = halfHeightA + halfHeightB;

			if (Math.Abs(xDist) >= minXDist || Math.Abs(yDist) >= minYDist)
				return Vector2.Zero;

			float xDepth = xDist > 0 ? minXDist - xDist : -minXDist - xDist;
			float yDepth = yDist > 0 ? minYDist - yDist : -minYDist - yDist;

			return new Vector2(xDepth, yDepth);
		}
	}
}
