using System;
using UnityEngine;

[Serializable]
public class LoadLevel : MonoBehaviour
{
	public GameObject LoadingIcon;

	private Transform Player;

	public int MyDist;

	public GameObject MyTerrain;

	public int KillDist;

	public string MyLevel;

	public string MyLoadedName;

	private GameObject LoadedLevel;

	private bool Loaded;

	private int Distance;

	private object MyTransform;

	private bool Delay;

	public LoadLevel()
	{
		this.MyDist = 300;
		this.KillDist = 500;
		this.Distance = 1000;
	}

	public override void Awake()
	{
		this.InvokeRepeating("CheckLoad", (float)UnityEngine.Random.Range(2, 5), (float)UnityEngine.Random.Range(2, 5));
		this.Player = GameObject.Find("player").transform;
	}

	public override void CheckLoad()
	{
		this.Distance = (int)Vector3.Distance(this.transform.position, this.Player.transform.position);
		if (this.Distance < this.MyDist && !this.Delay && !this.Loaded)
		{
			this.LoadingIcon.SetActive(true);
			this.Loaded = true;
			this.Delay = true;
			this.Invoke("ResetDelay", (float)30);
			AsyncOperation asyncOperation = Application.LoadLevelAdditiveAsync(this.MyLevel);
			this.MyTerrain.SetActive(true);
			this.Invoke("LinkLoad", (float)5);
		}
		else if (this.Loaded && this.Distance > this.KillDist && !this.Delay)
		{
			this.Loaded = false;
			this.LoadedLevel.SendMessage("Kill");
			this.MyTerrain.SetActive(false);
		}
	}

	public override void SendKill()
	{
		UnityEngine.Object.Destroy(this.LoadedLevel);
	}

	public override void ResetDelay()
	{
		this.Delay = false;
	}

	public override void LinkLoad()
	{
		this.LoadingIcon.SetActive(false);
		this.LoadedLevel = GameObject.Find(this.MyLoadedName);
	}

	public override void Main()
	{
	}
}
