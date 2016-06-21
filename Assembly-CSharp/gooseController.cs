using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class gooseController : MonoBehaviour
{
	public TheForestAtmosphere Atmos;

	public GameObject goose;

	public int spawnAmount;

	public Transform[] lakes;

	public Transform[] landingPoints;

	public Transform leader;

	public Transform currLandingPoint;

	public Vector3 landingPos;

	private bool takeOff;

	private bool initStart;

	private bool spawnDisabled;

	private bool replenish;

	private bool geeseOnWater;

	public List<GameObject> spawnedGeese = new List<GameObject>();

	public bool debugGeese;

	public float debugStartFlyTime;

	public float debugFlyTime;

	public float debugOnWaterTime;

	public GameObject forceLandingPoint;

	private Camera currentCamera;

	private Vector3 screenPos;

	private void Start()
	{
		this.Atmos = Scene.Atmosphere;
		this.currentCamera = Camera.main;
		if (this.spawnAmount > 0)
		{
			base.StartCoroutine("spawnGeese");
		}
		if (this.debugGeese)
		{
			base.Invoke("doTakeOff", this.debugStartFlyTime);
		}
		else
		{
			base.Invoke("doTakeOff", (float)UnityEngine.Random.Range(60, 120));
		}
		base.InvokeRepeating("updateGeese", 1f, 5f);
	}

	public void initFlying()
	{
		if (!this.initStart)
		{
			if (this.debugGeese)
			{
				base.Invoke("doTakeOff", this.debugOnWaterTime);
			}
			else
			{
				base.Invoke("doTakeOff", UnityEngine.Random.Range(120f, 200f));
			}
			this.initStart = true;
		}
	}

	private void startCoolDown()
	{
		this.initStart = false;
	}

	private void updateGeese()
	{
		if (Clock.InCave && !this.spawnDisabled)
		{
			this.disableGeese();
			this.spawnDisabled = true;
		}
		else if (!Clock.InCave && this.spawnDisabled)
		{
			base.StartCoroutine("spawnGeese");
			this.spawnDisabled = false;
		}
		if (this.spawnedGeese.Count < this.spawnAmount && !this.replenish && !Clock.InCave)
		{
			this.spawnedGeese.RemoveAll((GameObject o) => o == null);
			if (!this.spawnedGeese[0].GetComponent<newGooseAi>().flying)
			{
				base.StartCoroutine("spawnSingleGoose");
				base.Invoke("spawnCoolDown", 50f);
				this.replenish = true;
			}
		}
	}

	private void spawnCoolDown()
	{
		this.replenish = false;
	}

	private void disableGeese()
	{
		this.spawnedGeese.RemoveAll((GameObject o) => o == null);
		foreach (GameObject current in this.spawnedGeese)
		{
			if (current)
			{
				UnityEngine.Object.Destroy(current);
			}
		}
		this.spawnedGeese.Clear();
		this.leader = null;
	}

	[DebuggerHidden]
	public IEnumerator resetLeader()
	{
		gooseController.<resetLeader>c__Iterator5D <resetLeader>c__Iterator5D = new gooseController.<resetLeader>c__Iterator5D();
		<resetLeader>c__Iterator5D.<>f__this = this;
		return <resetLeader>c__Iterator5D;
	}

	private void resetTakeOff()
	{
		this.takeOff = false;
	}

	private void doTakeOff()
	{
		for (int i = 0; i < this.spawnedGeese.Count; i++)
		{
			if (this.spawnedGeese[i])
			{
				this.spawnedGeese[i].SendMessage("startFlying");
			}
		}
		this.initStart = false;
	}

	[DebuggerHidden]
	private IEnumerator spawnGeese()
	{
		gooseController.<spawnGeese>c__Iterator5E <spawnGeese>c__Iterator5E = new gooseController.<spawnGeese>c__Iterator5E();
		<spawnGeese>c__Iterator5E.<>f__this = this;
		return <spawnGeese>c__Iterator5E;
	}

	[DebuggerHidden]
	private IEnumerator spawnSingleGoose()
	{
		gooseController.<spawnSingleGoose>c__Iterator5F <spawnSingleGoose>c__Iterator5F = new gooseController.<spawnSingleGoose>c__Iterator5F();
		<spawnSingleGoose>c__Iterator5F.<>f__this = this;
		return <spawnSingleGoose>c__Iterator5F;
	}

	private bool pointOffCamera(Vector3 pos)
	{
		this.screenPos = this.currentCamera.WorldToViewportPoint(pos);
		return this.screenPos.x < 0f || this.screenPos.x > 1f || this.screenPos.y < 0f || this.screenPos.y > 1f;
	}

	private Vector2 RandomSpawnPos(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}
}
