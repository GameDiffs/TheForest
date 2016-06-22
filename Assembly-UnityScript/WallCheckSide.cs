using System;
using UnityEngine;

[Serializable]
public class WallCheckSide : MonoBehaviour
{
	public GameObject MyWalls;

	public bool Middle;

	private bool Off;

	public override void Start()
	{
		this.Invoke("TurnOffChecker", (float)2);
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Con") && !this.Off)
		{
			UnityEngine.Object.Destroy(other);
			if (!this.Middle)
			{
				UnityEngine.Object.Destroy(this.MyWalls);
			}
		}
	}

	public override void TurnOffChecker()
	{
		if (!(this.MyWalls == null))
		{
			this.Off = true;
			this.MyWalls.SetActive(true);
		}
		this.gameObject.SetActive(false);
	}

	public override void Main()
	{
	}
}
