using Boo.Lang.Runtime;
using System;
using UnityEngine;

public static class Drawing_tc1
{
	[Serializable]
	public class clip_class
	{
		public float u1;

		public float u2;

		public clip_class(float start, float end)
		{
			this.u1 = start;
			this.u2 = end;
		}
	}

	[Serializable]
	public class point_class
	{
		public Vector2 p1;

		public Vector2 p2;

		public point_class(Vector2 start, Vector2 end)
		{
			this.p1 = start;
			this.p2 = end;
		}
	}

	[NonSerialized]
	public static Texture2D aaLineTex;

	[NonSerialized]
	public static Texture2D lineTex;

	[NonSerialized]
	public static bool clippingEnabled;

	[NonSerialized]
	public static Rect clippingBounds;

	[NonSerialized]
	public static Material lineMaterial;

	static Drawing_tc1()
	{
		Drawing_tc1.$static_initializer$();
		Drawing_tc1.aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, true);
		Drawing_tc1.aaLineTex.SetPixel(0, 0, new Color((float)1, (float)1, (float)1, (float)0));
		Drawing_tc1.aaLineTex.SetPixel(0, 1, Color.white);
		Drawing_tc1.aaLineTex.SetPixel(0, 2, new Color((float)1, (float)1, (float)1, (float)0));
		Drawing_tc1.aaLineTex.Apply();
		Drawing_tc1.lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
		Drawing_tc1.lineTex.SetPixel(0, 1, Color.white);
		Drawing_tc1.lineTex.Apply();
	}

	public static void DrawLineMac(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
	{
		Color color2 = GUI.color;
		Matrix4x4 matrix = GUI.matrix;
		float num = width;
		if (antiAlias)
		{
			width *= (float)3;
		}
		float num2 = Vector3.Angle(pointB - pointA, Vector2.right) * (float)((pointA.y > pointB.y) ? -1 : 1);
		float magnitude = (pointB - pointA).magnitude;
		if (magnitude > 0.01f)
		{
			Vector3 vector = new Vector3(pointA.x, pointA.y, (float)0);
			Vector3 b = new Vector3((pointB.x - pointA.x) * 0.5f, (pointB.y - pointA.y) * 0.5f, (float)0);
			Vector3 b2 = Vector3.zero;
			if (antiAlias)
			{
				b2 = new Vector3(-num * 1.5f * Mathf.Sin(num2 * 0.0174532924f), num * 1.5f * Mathf.Cos(num2 * 0.0174532924f));
			}
			else
			{
				b2 = new Vector3(-num * 0.5f * Mathf.Sin(num2 * 0.0174532924f), num * 0.5f * Mathf.Cos(num2 * 0.0174532924f));
			}
			GUI.color = color;
			GUI.matrix = Drawing_tc1.translationMatrix(vector) * GUI.matrix;
			GUIUtility.ScaleAroundPivot(new Vector2(magnitude, width), new Vector2(-0.5f, (float)0));
			GUI.matrix = Drawing_tc1.translationMatrix(-vector) * GUI.matrix;
			GUIUtility.RotateAroundPivot(num2, Vector2.zero);
			GUI.matrix = Drawing_tc1.translationMatrix(vector - b2 - b) * GUI.matrix;
			if (antiAlias)
			{
				GUI.DrawTexture(new Rect((float)0, (float)0, (float)1, (float)1), Drawing_tc1.aaLineTex);
			}
			else
			{
				GUI.DrawTexture(new Rect((float)0, (float)0, (float)1, (float)1), Drawing_tc1.lineTex);
			}
		}
		GUI.matrix = matrix;
		GUI.color = color2;
	}

	public static void DrawLineWindows(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
	{
		Color color2 = GUI.color;
		Matrix4x4 matrix = GUI.matrix;
		if (antiAlias)
		{
			width *= (float)3;
		}
		float num = Vector3.Angle(pointB - pointA, Vector2.right) * (float)((pointA.y > pointB.y) ? -1 : 1);
		float magnitude = (pointB - pointA).magnitude;
		Vector3 vector = new Vector3(pointA.x, pointA.y, (float)0);
		GUI.color = color;
		GUI.matrix = Drawing_tc1.translationMatrix(vector) * GUI.matrix;
		GUIUtility.ScaleAroundPivot(new Vector2(magnitude, width), new Vector2(-0.5f, (float)0));
		GUI.matrix = Drawing_tc1.translationMatrix(-vector) * GUI.matrix;
		GUIUtility.RotateAroundPivot(num, new Vector2((float)0, (float)0));
		GUI.matrix = Drawing_tc1.translationMatrix(vector + new Vector3(width / (float)2, -magnitude / (float)2) * Mathf.Sin(num * 0.0174532924f)) * GUI.matrix;
		if (!antiAlias)
		{
			GUI.DrawTexture(new Rect((float)0, (float)0, (float)1, (float)1), Drawing_tc1.lineTex);
		}
		else
		{
			GUI.DrawTexture(new Rect((float)0, (float)0, (float)1, (float)1), Drawing_tc1.aaLineTex);
		}
		GUI.matrix = matrix;
		GUI.color = color2;
	}

	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias, Rect screen)
	{
		Drawing_tc1.clippingBounds = screen;
		Drawing_tc1.DrawLine(pointA, pointB, color, width);
	}

	public static void curveOutIn(Rect wr, Rect wr2, Color color, Color shadow, int width, Rect screen)
	{
		Drawing_tc1.BezierLine(new Vector2(wr.x + wr.width, wr.y + (float)width + wr.height / (float)2), new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / (float)2, wr.y + (float)width + wr.height / (float)2), new Vector2(wr2.x, wr2.y + (float)width + wr2.height / (float)2), new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / (float)2, wr2.y + (float)width + wr2.height / (float)2), shadow, (float)width, true, 20, screen);
		Drawing_tc1.BezierLine(new Vector2(wr.x + wr.width, wr.y + wr.height / (float)2), new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / (float)2, wr.y + wr.height / (float)2), new Vector2(wr2.x, wr2.y + wr2.height / (float)2), new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / (float)2, wr2.y + wr2.height / (float)2), color, (float)width, true, 20, screen);
	}

	public static void BezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments, Rect screen)
	{
		Vector2 pointA = Drawing_tc1.cubeBezier(start, startTangent, end, endTangent, (float)0);
		for (int i = 1; i <= segments; i++)
		{
			Vector2 vector = Drawing_tc1.cubeBezier(start, startTangent, end, endTangent, (float)(i / segments));
			Drawing_tc1.DrawLine(pointA, vector, color, width, antiAlias, screen);
			pointA = vector;
		}
	}

	public static Vector2 cubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
	{
		float d = (float)1 - t;
		return s * d * d * d + (float)3 * st * d * d * t + (float)3 * et * d * t * t + e * t * t * t;
	}

	public static Matrix4x4 translationMatrix(Vector3 v)
	{
		return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
	}

	public static bool clip_test(float p, float q, Drawing_tc1.clip_class u)
	{
		float num = 0f;
		bool result = true;
		if (p < (float)0)
		{
			num = q / p;
			if (num > u.u2)
			{
				result = false;
			}
			else if (num > u.u1)
			{
				u.u1 = num;
			}
		}
		else if (p > (float)0)
		{
			num = q / p;
			if (num < u.u1)
			{
				result = false;
			}
			else if (num < u.u2)
			{
				u.u2 = num;
			}
		}
		else if (q < (float)0)
		{
			result = false;
		}
		return result;
	}

	public static bool segment_rect_intersection(Rect bounds, Drawing_tc1.point_class p)
	{
		float num = p.p2.x - p.p1.x;
		float num2 = 0f;
		Drawing_tc1.clip_class clip_class = new Drawing_tc1.clip_class((float)0, 1f);
		int arg_173_0;
		if (Drawing_tc1.clip_test(-num, p.p1.x - bounds.xMin, clip_class) && Drawing_tc1.clip_test(num, bounds.xMax - p.p1.x, clip_class))
		{
			num2 = p.p2.y - p.p1.y;
			if (Drawing_tc1.clip_test(-num2, p.p1.y - bounds.yMin, clip_class) && Drawing_tc1.clip_test(num2, bounds.yMax - p.p1.y, clip_class))
			{
				if (clip_class.u2 < 1f)
				{
					p.p2.x = p.p1.x + clip_class.u2 * num;
					p.p2.y = p.p1.y + clip_class.u2 * num2;
				}
				if (clip_class.u1 > (float)0)
				{
					p.p1.x = p.p1.x + clip_class.u1 * num;
					p.p1.y = p.p1.y + clip_class.u1 * num2;
				}
				arg_173_0 = 1;
				return arg_173_0 != 0;
			}
		}
		arg_173_0 = 0;
		return arg_173_0 != 0;
	}

	public static void BeginGroup(Rect position)
	{
		Drawing_tc1.clippingEnabled = true;
		Drawing_tc1.clippingBounds = new Rect((float)0, (float)0, position.width, position.height);
		GUI.BeginGroup(position);
	}

	public static void EndGroup()
	{
		GUI.EndGroup();
		Drawing_tc1.clippingBounds = new Rect((float)0, (float)0, (float)Screen.width, (float)Screen.height);
		Drawing_tc1.clippingEnabled = false;
	}

	public static void CreateMaterial()
	{
		if (!(Drawing_tc1.lineMaterial != null))
		{
			Drawing_tc1.lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" + "SubShader { Pass { " + "    Blend SrcAlpha OneMinusSrcAlpha " + "    ZWrite Off Cull Off Fog { Mode Off } " + "    BindChannels {" + "      Bind \"vertex\", vertex Bind \"color\", color }" + "} } }");
			Drawing_tc1.lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			Drawing_tc1.lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
	{
		if (!RuntimeServices.EqualityOperator(Event.current, null))
		{
			if (Event.current.type == EventType.Repaint)
			{
				Drawing_tc1.point_class point_class = new Drawing_tc1.point_class(start, end);
				Drawing_tc1.CreateMaterial();
				Drawing_tc1.lineMaterial.SetPass(0);
				Vector3 vector = default(Vector3);
				Vector3 vector2 = default(Vector3);
				if (width == (float)1)
				{
					GL.Begin(1);
					GL.Color(color);
					vector = new Vector3(start.x, start.y, (float)0);
					vector2 = new Vector3(end.x, end.y, (float)0);
					GL.Vertex(vector);
					GL.Vertex(vector2);
				}
				else
				{
					GL.Begin(7);
					GL.Color(color);
					vector = new Vector3(end.y, start.x, (float)0);
					vector2 = new Vector3(start.y, end.x, (float)0);
					Vector3 b = (vector - vector2).normalized * width / 2f;
					Vector3 a = new Vector3(start.x, start.y, (float)0);
					Vector3 a2 = new Vector3(end.x, end.y, (float)0);
					GL.Vertex(a - b);
					GL.Vertex(a + b);
					GL.Vertex(a2 + b);
					GL.Vertex(a2 - b);
				}
				GL.End();
			}
		}
	}

	public static void DrawBox(Rect box, Color color, float width)
	{
		Vector2 vector = new Vector2(box.xMin, box.yMin);
		Vector2 vector2 = new Vector2(box.xMax, box.yMin);
		Vector2 vector3 = new Vector2(box.xMax, box.yMax);
		Vector2 vector4 = new Vector2(box.xMin, box.yMax);
		Drawing_tc1.DrawLine(vector, vector2, color, width);
		Drawing_tc1.DrawLine(vector2, vector3, color, width);
		Drawing_tc1.DrawLine(vector3, vector4, color, width);
		Drawing_tc1.DrawLine(vector4, vector, color, width);
	}

	public static void DrawBox(Vector2 topLeftCorner, Vector2 bottomRightCorner, Color color, float width)
	{
		Rect box = new Rect(topLeftCorner.x, topLeftCorner.y, bottomRightCorner.x - topLeftCorner.x, bottomRightCorner.y - topLeftCorner.y);
		Drawing_tc1.DrawBox(box, color, width);
	}

	public static void DrawRoundedBox(Rect box, float radius, Color color, float width)
	{
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		Vector2 vector3 = default(Vector2);
		Vector2 vector4 = default(Vector2);
		Vector2 vector5 = default(Vector2);
		Vector2 vector6 = default(Vector2);
		Vector2 vector7 = default(Vector2);
		Vector2 vector8 = default(Vector2);
		vector = new Vector2(box.xMin + radius, box.yMin);
		vector2 = new Vector2(box.xMax - radius, box.yMin);
		vector3 = new Vector2(box.xMax, box.yMin + radius);
		vector4 = new Vector2(box.xMax, box.yMax - radius);
		vector5 = new Vector2(box.xMax - radius, box.yMax);
		vector6 = new Vector2(box.xMin + radius, box.yMax);
		vector7 = new Vector2(box.xMin, box.yMax - radius);
		vector8 = new Vector2(box.xMin, box.yMin + radius);
		Drawing_tc1.DrawLine(vector, vector2, color, width);
		Drawing_tc1.DrawLine(vector3, vector4, color, width);
		Drawing_tc1.DrawLine(vector5, vector6, color, width);
		Drawing_tc1.DrawLine(vector7, vector8, color, width);
		Vector2 startTangent = default(Vector2);
		Vector2 endTangent = default(Vector2);
		float num = radius / (float)2;
		startTangent = new Vector2(vector8.x, vector8.y + num);
		endTangent = new Vector2(vector.x - num, vector.y);
		Drawing_tc1.DrawBezier(vector8, startTangent, vector, endTangent, color, width);
		startTangent = new Vector2(vector2.x + num, vector2.y);
		endTangent = new Vector2(vector3.x, vector3.y - num);
		Drawing_tc1.DrawBezier(vector2, startTangent, vector3, endTangent, color, width);
		startTangent = new Vector2(vector4.x, vector4.y + num);
		endTangent = new Vector2(vector5.x + num, vector5.y);
		Drawing_tc1.DrawBezier(vector4, startTangent, vector5, endTangent, color, width);
		startTangent = new Vector2(vector6.x - num, vector6.y);
		endTangent = new Vector2(vector7.x, vector7.y + num);
		Drawing_tc1.DrawBezier(vector6, startTangent, vector7, endTangent, color, width);
	}

	public static void DrawConnectingCurve(Vector2 start, Vector2 end, Color color, float width)
	{
		Vector2 a = start - end;
		Vector2 startTangent = start;
		startTangent.x -= (a / (float)2).x;
		Vector2 endTangent = end;
		endTangent.x += (a / (float)2).x;
		int segments = Mathf.FloorToInt(a.magnitude / (float)20 * (float)3);
		Drawing_tc1.DrawBezier(start, startTangent, end, endTangent, color, width, segments);
	}

	public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width)
	{
		int segments = Mathf.FloorToInt((start - end).magnitude / (float)20) * 3;
		Drawing_tc1.DrawBezier(start, startTangent, end, endTangent, color, width, segments);
	}

	public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments)
	{
		Vector2 start2 = Drawing_tc1.CubeBezier(start, startTangent, end, endTangent, (float)0);
		for (int i = 1; i <= segments; i++)
		{
			Vector2 vector = Drawing_tc1.CubeBezier(start, startTangent, end, endTangent, (float)(i / segments));
			Drawing_tc1.DrawLine(start2, vector, color, width);
			start2 = vector;
		}
	}

	public static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
	{
		float num = (float)1 - t;
		float num2 = num * t;
		return num * num * num * s + (float)3 * num * num2 * st + (float)3 * num2 * t * et + t * t * t * e;
	}

	private static void $static_initializer$()
	{
		Drawing_tc1.clippingEnabled = true;
	}
}
