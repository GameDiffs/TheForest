using System;
using UnityEngine;

public class gooseAvoidance : MonoBehaviour
{
	public GameObject ControllerGo;

	private newGooseAi ai;

	private bool startUp;

	private void Start()
	{
		this.ai = this.ControllerGo.GetComponent<newGooseAi>();
		base.Invoke("doStart", 1f);
	}

	private void doStart()
	{
		this.startUp = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.startUp)
		{
			return;
		}
		if (this.ai.flying)
		{
			return;
		}
		if (other.gameObject.CompareTag("animalCollide"))
		{
			Vector3 vector = base.transform.InverseTransformPoint(other.transform.position);
			if (vector.x > 0f && this.ai.turnInt != 1)
			{
				this.ai.turnInt = -1;
				this.ai.setCollisionDir();
			}
			else if (vector.x < 0f && this.ai.turnInt != -1)
			{
				this.ai.turnInt = 1;
				this.ai.setCollisionDir();
			}
		}
		if (this.ai.allPlayers.Count > 0 && other.gameObject.CompareTag("soundAlert") && Vector3.Distance(this.ai.allPlayers[0].transform.position, base.transform.position) < 13f)
		{
			this.ai.Invoke("fleeDirection", UnityEngine.Random.Range(0f, 0.4f));
		}
	}

	private void disableCloseTurn()
	{
		this.ai.turnInt = 0;
	}
}
