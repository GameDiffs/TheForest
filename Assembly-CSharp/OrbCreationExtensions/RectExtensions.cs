using System;
using UnityEngine;

namespace OrbCreationExtensions
{
	public static class RectExtensions
	{
		public static bool MouseInRect(this Rect rect, Vector2 point)
		{
			return rect.Contains(point);
		}

		public static bool MouseInRect(this Rect rect)
		{
			Vector2 point = Input.mousePosition;
			point.y = (float)Screen.height - point.y;
			return rect.MouseInRect(point);
		}

		public static bool MouseInRect(this Rect rect, Rect parentRect, Vector2 point)
		{
			rect.x += parentRect.x;
			rect.y += parentRect.y;
			return rect.MouseInRect(point);
		}

		public static bool MouseInRect(this Rect rect, Rect parentRect)
		{
			Vector2 point = Input.mousePosition;
			point.y = (float)Screen.height - point.y;
			return rect.MouseInRect(parentRect, point);
		}

		public static bool MouseInRect(this Rect rect, Rect parentRect1, Rect parentRect2, Vector2 point)
		{
			rect.x += parentRect1.x;
			rect.y += parentRect1.y;
			rect.x += parentRect2.x;
			rect.y += parentRect2.y;
			return rect.MouseInRect(point);
		}

		public static bool MouseInRect(this Rect rect, Rect parentRect1, Rect parentRect2)
		{
			Vector2 point = Input.mousePosition;
			point.y = (float)Screen.height - point.y;
			return rect.MouseInRect(parentRect1, parentRect2, point);
		}

		public static Vector2 RelativeMousePosInRect(this Rect rect, Vector2 point)
		{
			Vector2 result = new Vector2(-1f, -1f);
			if (rect.Contains(point))
			{
				result.x = point.x - rect.x;
				if (rect.width > 0f)
				{
					result.x = Mathf.Abs(result.x / rect.width);
				}
				result.y = point.y - rect.y;
				if (rect.height > 0f)
				{
					result.y = 1f - Mathf.Abs(result.y / rect.height);
				}
			}
			return result;
		}

		public static Vector2 RelativeMousePosInRect(this Rect rect)
		{
			Vector2 point = Input.mousePosition;
			point.y = (float)Screen.height - point.y;
			return rect.RelativeMousePosInRect(point);
		}

		public static Rect RelativeRectInImage(this Rect r, Texture2D img)
		{
			return new Rect(r.x / (float)img.width, 1f - (r.y + r.height) / (float)img.height, r.width / (float)img.width, r.height / (float)img.height);
		}

		public static float MaxExtents(this Bounds b)
		{
			return Mathf.Max(Mathf.Max(b.extents.x, b.extents.y), b.extents.z);
		}

		public static float MaxSize(this Bounds b)
		{
			return Mathf.Max(Mathf.Max(b.size.x, b.size.y), b.size.z);
		}

		public static float MinExtents(this Bounds b)
		{
			return Mathf.Min(Mathf.Min(b.extents.x, b.extents.y), b.extents.z);
		}

		public static float MinSize(this Bounds b)
		{
			return Mathf.Min(Mathf.Min(b.size.x, b.size.y), b.size.z);
		}
	}
}
