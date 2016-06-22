using System;
using UnityEngine;

[Serializable]
public class CutBush : MonoBehaviour
{
	private int Health;

	private GameObject Burst;

	public CutBush()
	{
		this.Health = 4;
	}

	public override void OnEnable()
	{
		this.Health = LOD_Stats.GetInt(this, 4);
	}

	public override void OnDisable()
	{
		LOD_Stats.SetInt(this, this.Health);
	}

	public override void Hit()
	{
		this.Burst = (UnityEngine.Object.Instantiate(Resources.Load("LeavesBurst"), this.transform.position, this.transform.rotation) as GameObject);
		float y = this.Burst.transform.localPosition.y + (float)4;
		Vector3 localPosition = this.Burst.transform.localPosition;
		float num = localPosition.y = y;
		Vector3 vector = this.Burst.transform.localPosition = localPosition;
		this.Health--;
		if (this.Health <= 0)
		{
			this.CutDown();
		}
	}

	public override void CutDown()
	{
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
