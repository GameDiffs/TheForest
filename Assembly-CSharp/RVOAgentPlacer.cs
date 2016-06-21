using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class RVOAgentPlacer : MonoBehaviour
{
	private const float rad2Deg = 57.2957764f;

	public int agents = 100;

	public float ringSize = 100f;

	public LayerMask mask;

	public GameObject prefab;

	public Vector3 goalOffset;

	public float repathRate = 1f;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		RVOAgentPlacer.<Start>c__IteratorA <Start>c__IteratorA = new RVOAgentPlacer.<Start>c__IteratorA();
		<Start>c__IteratorA.<>f__this = this;
		return <Start>c__IteratorA;
	}

	public Color GetColor(float angle)
	{
		return RVOAgentPlacer.HSVToRGB(angle * 57.2957764f, 0.8f, 0.6f);
	}

	private static Color HSVToRGB(float h, float s, float v)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = s * v;
		float num5 = h / 60f;
		float num6 = num4 * (1f - Math.Abs(num5 % 2f - 1f));
		if (num5 < 1f)
		{
			num = num4;
			num2 = num6;
		}
		else if (num5 < 2f)
		{
			num = num6;
			num2 = num4;
		}
		else if (num5 < 3f)
		{
			num2 = num4;
			num3 = num6;
		}
		else if (num5 < 4f)
		{
			num2 = num6;
			num3 = num4;
		}
		else if (num5 < 5f)
		{
			num = num6;
			num3 = num4;
		}
		else if (num5 < 6f)
		{
			num = num4;
			num3 = num6;
		}
		float num7 = v - num4;
		num += num7;
		num2 += num7;
		num3 += num7;
		return new Color(num, num2, num3);
	}
}
