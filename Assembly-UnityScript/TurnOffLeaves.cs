using System;
using UnityEngine;

[Serializable]
public class TurnOffLeaves : MonoBehaviour
{
	public GameObject Leaves;

	public GameObject Rig;

	public GameObject Lower;

	public GameObject Upper;

	public GameObject Particles;

	public Transform PosChecker;

	public GameObject destroyMe;

	public float logSwitchHeightOffset;

	public float logSwitchDelay;

	public float posCheckHeight;

	public float terrainHeight;

	private bool delayStart;

	private bool Off;

	public TurnOffLeaves()
	{
		this.logSwitchHeightOffset = (float)5;
		this.logSwitchDelay = 0.2f;
	}

	public override void Awake()
	{
		if (BoltNetwork.isClient)
		{
			this.enabled = false;
		}
		else
		{
			this.Invoke("doDelayStart", (float)1);
			this.Invoke("TurnOff", (float)13);
		}
	}

	public override void doDelayStart()
	{
		this.delayStart = true;
	}

	public override void Update()
	{
		if (!BoltNetwork.isClient)
		{
			if (this.delayStart)
			{
				if (!this.Off)
				{
					this.terrainHeight = Terrain.activeTerrain.SampleHeight(this.PosChecker.position) + Terrain.activeTerrain.transform.position.y + this.logSwitchHeightOffset;
					this.posCheckHeight = this.PosChecker.position.y;
					if (this.PosChecker.position.y < this.terrainHeight)
					{
						this.Off = true;
						this.Invoke("TurnOff", this.logSwitchDelay);
					}
				}
			}
		}
	}

	public override void TurnOff()
	{
		if (!BoltNetwork.isClient)
		{
			this.ActivateLeafParticles();
			if (this.Lower)
			{
				this.Lower.transform.DetachChildren();
			}
			if (this.Upper)
			{
				this.Upper.transform.DetachChildren();
			}
			if (this.destroyMe)
			{
				UnityEngine.Object.Destroy(this.destroyMe);
			}
			UnityEngine.Object.Destroy(this.transform.parent.gameObject);
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("soundDetectGo 1"), this.transform.position, this.transform.rotation);
		gameObject.SendMessage("setRange", 150);
		this.Particles = null;
		this.PosChecker = null;
		this.Lower = null;
		this.Upper = null;
		this.destroyMe = null;
	}

	public override void ActivateLeafParticles()
	{
		if (this.Particles)
		{
			this.Particles.SetActive(true);
			this.Particles.transform.parent = null;
		}
	}

	public override void Main()
	{
	}
}
