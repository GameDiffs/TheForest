using System;
using UnityEngine;

public class SSAOMovementTest : MonoBehaviour
{
	private Vector3 randomMovment;

	private Vector3 randomRotation;

	private Vector3 startPosition;

	private float randomTimeOffset;

	private void Start()
	{
		this.startPosition = base.transform.position;
		this.randomTimeOffset = UnityEngine.Random.Range(0f, 100f);
		this.randomMovment = Vector3.Normalize(new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)) * 0.1f;
		this.randomRotation = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
	}

	private void Update()
	{
		base.transform.position = this.startPosition + this.randomMovment * Mathf.Sin(Time.timeSinceLevelLoad + this.randomTimeOffset);
		base.transform.eulerAngles = this.randomRotation * (Time.timeSinceLevelLoad + this.randomTimeOffset) * 3.14159274f * 2f;
	}
}
