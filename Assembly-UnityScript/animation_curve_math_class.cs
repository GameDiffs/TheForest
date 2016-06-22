using System;
using UnityEngine;

[Serializable]
public class animation_curve_math_class
{
	public override AnimationCurve set_curve_linear(AnimationCurve curve)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		for (int i = 0; i < curve.keys.Length; i++)
		{
			float inTangent = (float)0;
			float outTangent = (float)0;
			bool flag = false;
			bool flag2 = false;
			Vector2 b = default(Vector2);
			Vector2 a = default(Vector2);
			Vector2 vector = default(Vector2);
			Keyframe key = curve[i];
			if (i == 0)
			{
				inTangent = (float)0;
				flag = true;
			}
			if (i == curve.keys.Length - 1)
			{
				outTangent = (float)0;
				flag2 = true;
			}
			if (!flag)
			{
				b.x = curve.keys[i - 1].time;
				b.y = curve.keys[i - 1].value;
				a.x = curve.keys[i].time;
				a.y = curve.keys[i].value;
				vector = a - b;
				inTangent = vector.y / vector.x;
			}
			if (!flag2)
			{
				b.x = curve.keys[i].time;
				b.y = curve.keys[i].value;
				a.x = curve.keys[i + 1].time;
				a.y = curve.keys[i + 1].value;
				vector = a - b;
				outTangent = vector.y / vector.x;
			}
			key.inTangent = inTangent;
			key.outTangent = outTangent;
			animationCurve.AddKey(key);
		}
		return animationCurve;
	}
}
