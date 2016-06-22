using System;
using UnityEngine;

[Serializable]
public class CaveDistanceCheck : MonoBehaviour
{
	public GameObject Sky;

	public Terrain MainTerrain;

	private Transform Player;

	private int distance;

	private Transform MyLocation;

	public CaveDistanceCheck()
	{
		this.distance = 1;
	}

	public override void Awake()
	{
		this.Player = GameObject.Find("Player").transform;
		this.MyLocation = this.transform;
	}

	public override void Update()
	{
		this.CheckDistance();
	}

	public override void CheckDistance()
	{
		MonoBehaviour.print("JustCheckedDistance" + this.distance);
		this.distance = (int)Vector3.Distance(this.MyLocation.position, this.Player.position);
		if (this.distance <= 1)
		{
			this.MainTerrain.GetComponent<Collider>().enabled = false;
			this.Sky.SendMessage("GoDark");
		}
		else
		{
			this.MainTerrain.GetComponent<Collider>().enabled = false;
			this.Sky.SendMessage("GoLight");
		}
	}

	public override void Main()
	{
	}
}
