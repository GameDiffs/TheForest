using System;
using UnityEngine;

public class PlayerVis : MonoBehaviour
{
	public sceneTracker Ai;

	private float angle;

	private float SeenAmount;

	public GameObject Right;

	public GameObject Left;

	public GameObject Front;

	public void ShowSeenAngle()
	{
		base.CancelInvoke("TurnOffSeenAngle");
		this.angle = Vector3.Angle(this.Ai.playerVisDir, base.transform.forward);
		if (this.angle < 45f || this.angle > 315f)
		{
			this.Right.SetActive(true);
		}
		else if (this.angle > 45f && this.angle < 135f)
		{
			this.Front.SetActive(true);
		}
		else if (this.angle > 135f && this.angle < 225f)
		{
			this.Left.SetActive(true);
		}
		base.Invoke("TurnOffSeenAngle", 1f);
	}

	private void TurnOffSeenAngle()
	{
		this.Right.SetActive(false);
		this.Left.SetActive(false);
		this.Front.SetActive(false);
	}
}
