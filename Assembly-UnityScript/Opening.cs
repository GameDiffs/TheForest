using System;
using UnityEngine;

[Serializable]
public class Opening : MonoBehaviour
{
	public int LoadWaitTime;

	public int ExplodeSfxTime;

	public GameObject LoadingScreen;

	public GameObject Crash;

	public GameObject CurtainsAndWall;

	public Opening()
	{
		this.LoadWaitTime = 45;
		this.ExplodeSfxTime = 40;
	}

	public override void Start()
	{
		this.Invoke("StartGame", (float)this.LoadWaitTime);
		this.Invoke("Explosion", (float)this.ExplodeSfxTime);
	}

	public override void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.StartGame();
		}
	}

	public override void Explosion()
	{
		this.CurtainsAndWall.SetActive(false);
		this.Crash.SetActive(true);
	}

	public override void StartGame()
	{
		Application.LoadLevel("ForestMain_v07");
		this.LoadingScreen.SetActive(true);
	}

	public override void Main()
	{
	}
}
