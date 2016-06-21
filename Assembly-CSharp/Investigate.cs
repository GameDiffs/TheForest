using System;
using UnityEngine;

public class Investigate : MonoBehaviour
{
	[NonSerialized]
	public float InvestigationAmount;

	[Range(1f, 10f)]
	public float InvestigateDelay = 5f;

	[Range(0f, 0.25f)]
	public float MotionTollerance = 0.1f;

	[Range(1f, 10f)]
	public float TransitionSpeed = 5f;

	private float timer;

	private Vector3 lastPosition;

	private void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = position - this.lastPosition;
		this.lastPosition = position;
		float num = vector.magnitude / Time.deltaTime;
		bool flag = num <= this.MotionTollerance;
		if (flag)
		{
			this.timer += Time.deltaTime;
		}
		else
		{
			this.timer = 0f;
		}
		bool flag2 = this.timer >= this.InvestigateDelay;
		this.InvestigationAmount = Mathf.Lerp(this.InvestigationAmount, (!flag2) ? 0f : 1f, Time.deltaTime * this.TransitionSpeed);
		Shader.SetGlobalFloat("GlobalSheenIntensity", this.InvestigationAmount);
	}
}
