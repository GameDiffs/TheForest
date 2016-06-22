using System;
using UnityEngine;

[Serializable]
public class CutTreeSmall : MonoBehaviour
{
	public GameObject Sticks;

	public int Health;

	public CutTreeSmall()
	{
		this.Health = 2;
	}

	public override void Hit()
	{
		this.Health--;
		if (this.Health <= 0)
		{
			this.CutDown();
		}
	}

	public override void CutDown()
	{
		this.Sticks.SetActive(true);
		this.Sticks.transform.parent = null;
		UnityEngine.Object.Destroy(this.gameObject);
	}

	public override void Main()
	{
	}
}
