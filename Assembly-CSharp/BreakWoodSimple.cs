using Bolt;
using System;
using TheForest.World;
using UnityEngine;

public class BreakWoodSimple : MonoBehaviour
{
	public int Health;

	public GameObject Cut1;

	public GameObject Cut2;

	public GameObject Cut3;

	[Header("FMOD")]
	public string breakEvent = "event:/physics/wood/wood_small_break";

	private float LastHitTime;

	private bool breakEventPlayed;

	private void Hit(int damage)
	{
		this.Health -= damage;
		if (this.Health <= 0)
		{
			if (BoltNetwork.isRunning)
			{
				BreakPlank breakPlank = BreakPlank.Create(GlobalTargets.OnlyServer);
				breakPlank.Index = CoopWoodPlanks.Instance.GetIndex(this);
				breakPlank.Send();
			}
			else
			{
				this.CutDown();
			}
		}
	}

	private void Explosion()
	{
		this.Hit(this.Health);
	}

	public void CutDown()
	{
		if (!this.breakEventPlayed)
		{
			FMODCommon.PlayOneshot(this.breakEvent, base.transform);
			this.breakEventPlayed = true;
		}
		this.Cut1.SetActive(true);
		this.Cut2.SetActive(true);
		this.Cut3.SetActive(true);
		this.Cut1.transform.parent = null;
		this.Cut2.transform.parent = null;
		this.Cut3.transform.parent = null;
		this.Cut1.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		this.Cut2.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		this.Cut3.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100), (float)UnityEngine.Random.Range(30, 100));
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void LocalizedHit(LocalizedHitData data)
	{
		if (this.LastHitTime + 0.5f < Time.realtimeSinceStartup)
		{
			this.LastHitTime = Time.realtimeSinceStartup;
			Renderer[] componentsInChildren = base.transform.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].enabled)
				{
					Transform transform = componentsInChildren[i].transform;
					GameObject gameObject = transform.gameObject;
					if (Vector3.Distance(transform.position, data._position) < 4f)
					{
						transform.localRotation *= Quaternion.Euler((float)UnityEngine.Random.Range(-1, 1), (float)UnityEngine.Random.Range(-1, 1), (float)UnityEngine.Random.Range(-1, 1));
					}
				}
			}
		}
	}
}
