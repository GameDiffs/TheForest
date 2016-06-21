using System;
using UnityEngine;

public class Digger : MonoBehaviour
{
	public GameObject SpecialItem;

	public GameObject Rock;

	public string DirtHitEvent;

	public GameObject Burst;

	private int Dice;

	private int Dice2;

	private RaycastHit hit;

	private int layer;

	private int layerMask;

	private void Awake()
	{
		this.layer = 26;
		this.layerMask = 1 << this.layer;
	}

	public void doDig()
	{
		if (Physics.Raycast(base.transform.position, base.transform.up, out this.hit, 20f, this.layerMask) && this.hit.collider.CompareTag("TerrainMain"))
		{
			FMOD_StudioSystem.instance.PlayOneShot(this.DirtHitEvent, base.transform.position, null);
			this.Dice = UnityEngine.Random.Range(0, 4);
			this.Dice2 = UnityEngine.Random.Range(0, 20);
			if (this.Dice == 1)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.Rock, this.hit.point, base.transform.rotation) as GameObject;
				gameObject.transform.eulerAngles = new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
			}
			if (this.Dice2 == 1)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(this.SpecialItem, this.hit.point, base.transform.rotation) as GameObject;
				gameObject2.transform.eulerAngles = new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
	}

	private void Update()
	{
	}
}
