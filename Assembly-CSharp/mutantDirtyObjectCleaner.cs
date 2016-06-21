using System;
using UnityEngine;

public class mutantDirtyObjectCleaner : MonoBehaviour
{
	public GameObject[] leaderProps;

	public GameObject[] weapons;

	public GameObject tennisBelt;

	public GameObject fireStick;

	public GameObject fireBomb;

	private mutantAI_net aiNet;

	private void Start()
	{
		this.aiNet = base.transform.GetComponent<mutantAI_net>();
		base.Invoke("cleanUpDirty", 2f);
	}

	private void cleanUpDirty()
	{
		if (!this.aiNet.leader)
		{
			GameObject[] array = this.leaderProps;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i];
				if (gameObject)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}
		if (this.aiNet.femaleSkinny || this.aiNet.maleSkinny || this.aiNet.pale)
		{
			GameObject[] array2 = this.weapons;
			for (int j = 0; j < array2.Length; j++)
			{
				GameObject gameObject2 = array2[j];
				if (gameObject2)
				{
					UnityEngine.Object.Destroy(gameObject2);
				}
			}
		}
	}
}
