using System;
using UnityEngine;

public static class Drawing_tc
{
	private static Texture2D _aaLineTex;

	private static Texture2D _lineTex;

	private static bool clippingEnabled = true;

	private static Rect clippingBounds;

	public static Material lineMaterial;

	private static Texture2D adLineTex
	{
		get
		{
			if (!Drawing_tc._aaLineTex)
			{
				Drawing_tc._aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, true);
				Drawing_tc._aaLineTex.SetPixel(0, 0, new Color(1f, 1f, 1f, 0f));
				Drawing_tc._aaLineTex.SetPixel(0, 1, Color.white);
				Drawing_tc._aaLineTex.SetPixel(0, 2, new Color(1f, 1f, 1f, 0f));
				Drawing_tc._aaLineTex.Apply();
			}
			return Drawing_tc._aaLineTex;
		}
	}

	private static Texture2D lineTex
	{
		get
		{
			if (!Drawing_tc._lineTex)
			{
				Drawing_tc._lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
				Drawing_tc._lineTex.SetPixel(0, 1, Color.white);
				Drawing_tc._lineTex.Apply();
			}
			return Drawing_tc._lineTex;
		}
	}

	private static void DrawLineMac(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
	{
		Color color2 = GUI.color;
		Matrix4x4 matrix = GUI.matrix;
		float num = width;
		if (antiAlias)
		{
			width *= 3f;
		}
		float num2 = Vector3.Angle(pointB - pointA, Vector2.right) * (float)((pointA.y > pointB.y) ? -1 : 1);
		float magnitude = (pointB - pointA).magnitude;
		if (magnitude > 0.01f)
		{
			Vector3 vector = new Vector3(pointA.x, pointA.y, 0f);
			Vector3 b = new Vector3((pointB.x - pointA.x) * 0.5f, (pointB.y - pointA.y) * 0.5f, 0f);
			Vector3 zero = Vector3.zero;
			if (antiAlias)
			{
				zero = new Vector3(-num * 1.5f * Mathf.Sin(num2 * 0.0174532924f), num * 1.5f * Mathf.Cos(num2 * 0.0174532924f));
			}
			else
			{
				zero = new Vector3(-num * 0.5f * Mathf.Sin(num2 * 0.0174532924f), num * 0.5f * Mathf.Cos(num2 * 0.0174532924f));
			}
			GUI.color = color;
			GUI.matrix = Drawing_tc.translationMatrix(vector) * GUI.matrix;
			GUIUtility.ScaleAroundPivot(new Vector2(magnitude, width), new Vector2(-0.5f, 0f));
			GUI.matrix = Drawing_tc.translationMatrix(-vector) * GUI.matrix;
			GUIUtility.RotateAroundPivot(num2, Vector2.zero);
			GUI.matrix = Drawing_tc.translationMatrix(vector - zero - b) * GUI.matrix;
			GUI.DrawTexture(new Rect(0f, 0f, 1f, 1f), (!antiAlias) ? Drawing_tc.lineTex : Drawing_tc.adLineTex);
		}
		GUI.matrix = matrix;
		GUI.color = color2;
	}

	private static void DrawLineWindows(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
	{
		Color color2 = GUI.color;
		Matrix4x4 matrix = GUI.matrix;
		if (antiAlias)
		{
			width *= 3f;
		}
		float num = Vector3.Angle(pointB - pointA, Vector2.right) * (float)((pointA.y > pointB.y) ? -1 : 1);
		float magnitude = (pointB - pointA).magnitude;
		Vector3 vector = new Vector3(pointA.x, pointA.y, 0f);
		GUI.color = color;
		GUI.matrix = Drawing_tc.translationMatrix(vector) * GUI.matrix;
		GUIUtility.ScaleAroundPivot(new Vector2(magnitude, width), new Vector2(-0.5f, 0f));
		GUI.matrix = Drawing_tc.translationMatrix(-vector) * GUI.matrix;
		GUIUtility.RotateAroundPivot(num, new Vector2(0f, 0f));
		GUI.matrix = Drawing_tc.translationMatrix(vector + new Vector3(width / 2f, -magnitude / 2f) * Mathf.Sin(num * 0.0174532924f)) * GUI.matrix;
		GUI.DrawTexture(new Rect(0f, 0f, 1f, 1f), antiAlias ? Drawing_tc.adLineTex : Drawing_tc.lineTex);
		GUI.matrix = matrix;
		GUI.color = color2;
	}

	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias, Rect screen)
	{
		Drawing_tc.clippingBounds = screen;
		Drawing_tc.DrawLine(pointA, pointB, color, width);
	}

	public static void curveOutIn(Rect wr, Rect wr2, Color color, Color shadow, int width, Rect screen)
	{
		Drawing_tc.BezierLine(new Vector2(wr.x + wr.width, wr.y + (float)width + wr.height / 2f), new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2f, wr.y + (float)width + wr.height / 2f), new Vector2(wr2.x, wr2.y + (float)width + wr2.height / 2f), new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2f, wr2.y + (float)width + wr2.height / 2f), shadow, (float)width, true, 20, screen);
		Drawing_tc.BezierLine(new Vector2(wr.x + wr.width, wr.y + wr.height / 2f), new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2f, wr.y + wr.height / 2f), new Vector2(wr2.x, wr2.y + wr2.height / 2f), new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2f, wr2.y + wr2.height / 2f), color, (float)width, true, 20, screen);
	}

	public static void BezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments, Rect screen)
	{
		Vector2 pointA = Drawing_tc.cubeBezier(start, startTangent, end, endTangent, 0f);
		for (int i = 1; i <= segments; i++)
		{
			Vector2 vector = Drawing_tc.cubeBezier(start, startTangent, end, endTangent, (float)i / (float)segments);
			Drawing_tc.DrawLine(pointA, vector, color, width, antiAlias, screen);
			pointA = vector;
		}
	}

	private static Vector2 cubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
	{
		float d = 1f - t;
		return s * d * d * d + 3f * st * d * d * t + 3f * et * d * t * t + e * t * t * t;
	}

	private static Matrix4x4 translationMatrix(Vector3 v)
	{
		return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
	}

	private static bool clip_test(float p, float q, ref float u1, ref float u2)
	{
		bool result = true;
		if ((double)p < 0.0)
		{
			float num = q / p;
			if (num > u2)
			{
				result = false;
			}
			else if (num > u1)
			{
				u1 = num;
			}
		}
		else if ((double)p > 0.0)
		{
			float num = q / p;
			if (num < u1)
			{
				result = false;
			}
			else if (num < u2)
			{
				u2 = num;
			}
		}
		else if ((double)q < 0.0)
		{
			result = false;
		}
		return result;
	}

	private static bool segment_rect_intersection(Rect bounds, ref Vector2 p1, ref Vector2 p2)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = p2.x - p1.x;
		if (Drawing_tc.clip_test(-num3, p1.x - bounds.xMin, ref num, ref num2) && Drawing_tc.clip_test(num3, bounds.xMax - p1.x, ref num, ref num2))
		{
			float num4 = p2.y - p1.y;
			if (Drawing_tc.clip_test(-num4, p1.y - bounds.yMin, ref num, ref num2) && Drawing_tc.clip_test(num4, bounds.yMax - p1.y, ref num, ref num2))
			{
				if ((double)num2 < 1.0)
				{
					p2.x = p1.x + num2 * num3;
					p2.y = p1.y + num2 * num4;
				}
				if ((double)num > 0.0)
				{
					p1.x += num * num3;
					p1.y += num * num4;
				}
				return true;
			}
		}
		return false;
	}

	public static void BeginGroup(Rect position)
	{
		Drawing_tc.clippingEnabled = true;
		Drawing_tc.clippingBounds = new Rect(0f, 0f, position.width, position.height);
		GUI.BeginGroup(position);
	}

	public static void EndGroup()
	{
		GUI.EndGroup();
		Drawing_tc.clippingBounds = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		Drawing_tc.clippingEnabled = false;
	}

	public static void CreateMaterial()
	{
		if (Drawing_tc.lineMaterial != null)
		{
			return;
		}
		Drawing_tc.lineMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {     Blend SrcAlpha OneMinusSrcAlpha     ZWrite Off Cull Off Fog { Mode Off }     BindChannels {      Bind \"vertex\", vertex Bind \"color\", color }} } }");
		Drawing_tc.lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		Drawing_tc.lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	}

	public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
	{
		if (Event.current == null)
		{
			return;
		}
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (Drawing_tc.clippingEnabled && !Drawing_tc.segment_rect_intersection(Drawing_tc.clippingBounds, ref start, ref end))
		{
			return;
		}
		Drawing_tc.CreateMaterial();
		Drawing_tc.lineMaterial.SetPass(0);
		if (width == 1f)
		{
			GL.Begin(1);
			GL.Color(color);
			Vector3 vector = new Vector3(start.x, start.y, 0f);
			Vector3 vector2 = new Vector3(end.x, end.y, 0f);
			GL.Vertex(vector);
			GL.Vertex(vector2);
		}
		else
		{
			GL.Begin(7);
			GL.Color(color);
			Vector3 vector = new Vector3(end.y, start.x, 0f);
			Vector3 vector2 = new Vector3(start.y, end.x, 0f);
			Vector3 b = (vector - vector2).normalized * width / 2f;
			Vector3 a = new Vector3(start.x, start.y, 0f);
			Vector3 a2 = new Vector3(end.x, end.y, 0f);
			GL.Vertex(a - b);
			GL.Vertex(a + b);
			GL.Vertex(a2 + b);
			GL.Vertex(a2 - b);
		}
		GL.End();
	}

	public static void DrawBox(Rect box, Color color, float width)
	{
		Vector2 vector = new Vector2(box.xMin, box.yMin);
		Vector2 vector2 = new Vector2(box.xMax, box.yMin);
		Vector2 vector3 = new Vector2(box.xMax, box.yMax);
		Vector2 vector4 = new Vector2(box.xMin, box.yMax);
		Drawing_tc.DrawLine(vector, vector2, color, width);
		Drawing_tc.DrawLine(vector2, vector3, color, width);
		Drawing_tc.DrawLine(vector3, vector4, color, width);
		Drawing_tc.DrawLine(vector4, vector, color, width);
	}

	public static void DrawBox(Vector2 topLeftCorner, Vector2 bottomRightCorner, Color color, float width)
	{
		Rect box = new Rect(topLeftCorner.x, topLeftCorner.y, bottomRightCorner.x - topLeftCorner.x, bottomRightCorner.y - topLeftCorner.y);
		Drawing_tc.DrawBox(box, color, width);
	}

	public static void DrawRoundedBox(Rect box, float radius, Color color, float width)
	{
		Vector2 vector = new Vector2(box.xMin + radius, box.yMin);
		Vector2 vector2 = new Vector2(box.xMax - radius, box.yMin);
		Vector2 vector3 = new Vector2(box.xMax, box.yMin + radius);
		Vector2 vector4 = new Vector2(box.xMax, box.yMax - radius);
		Vector2 vector5 = new Vector2(box.xMax - radius, box.yMax);
		Vector2 vector6 = new Vector2(box.xMin + radius, box.yMax);
		Vector2 vector7 = new Vector2(box.xMin, box.yMax - radius);
		Vector2 vector8 = new Vector2(box.xMin, box.yMin + radius);
		Drawing_tc.DrawLine(vector, vector2, color, width);
		Drawing_tc.DrawLine(vector3, vector4, color, width);
		Drawing_tc.DrawLine(vector5, vector6, color, width);
		Drawing_tc.DrawLine(vector7, vector8, color, width);
		float num = radius / 2f;
		Vector2 startTangent = new Vector2(vector8.x, vector8.y + num);
		Vector2 endTangent = new Vector2(vector.x - num, vector.y);
		Drawing_tc.DrawBezier(vector8, startTangent, vector, endTangent, color, width);
		startTangent = new Vector2(vector2.x + num, vector2.y);
		endTangent = new Vector2(vector3.x, vector3.y - num);
		Drawing_tc.DrawBezier(vector2, startTangent, vector3, endTangent, color, width);
		startTangent = new Vector2(vector4.x, vector4.y + num);
		endTangent = new Vector2(vector5.x + num, vector5.y);
		Drawing_tc.DrawBezier(vector4, startTangent, vector5, endTangent, color, width);
		startTangent = new Vector2(vector6.x - num, vector6.y);
		endTangent = new Vector2(vector7.x, vector7.y + num);
		Drawing_tc.DrawBezier(vector6, startTangent, vector7, endTangent, color, width);
	}

	public static void DrawConnectingCurve(Vector2 start, Vector2 end, Color color, float width)
	{
		Vector2 a = start - end;
		Vector2 startTangent = start;
		startTangent.x -= (a / 2f).x;
		Vector2 endTangent = end;
		endTangent.x += (a / 2f).x;
		int segments = Mathf.FloorToInt(a.magnitude / 20f * 3f);
		Drawing_tc.DrawBezier(start, startTangent, end, endTangent, color, width, segments);
	}

	public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width)
	{
		int segments = Mathf.FloorToInt((start - end).magnitude / 20f) * 3;
		Drawing_tc.DrawBezier(start, startTangent, end, endTangent, color, width, segments);
	}

	public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments)
	{
		Vector2 start2 = Drawing_tc.CubeBezier(start, startTangent, end, endTangent, 0f);
		for (int i = 1; i <= segments; i++)
		{
			Vector2 vector = Drawing_tc.CubeBezier(start, startTangent, end, endTangent, (float)i / (float)segments);
			Drawing_tc.DrawLine(start2, vector, color, width);
			start2 = vector;
		}
	}

	private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
	{
		float num = 1f - t;
		float num2 = num * t;
		return num * num * num * s + 3f * num * num2 * st + 3f * num2 * t * et + t * t * t * e;
	}
}
