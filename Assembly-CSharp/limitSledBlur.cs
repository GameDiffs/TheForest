using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class limitSledBlur : MonoBehaviour
{
	public AmplifyMotionObjectBase[] amplifyBase;

	public bool doAmplify;

	public bool doAmplifyVel;

	private Rigidbody rb;

	private Transform thisTr;

	private float playerDist;

	private float vel;

	public List<GameObject> allPlayers = new List<GameObject>();

	public float limitDistance = 25f;

	private Transform playerTr;

	private void Start()
	{
		this.rb = base.transform.GetComponent<Rigidbody>();
		this.thisTr = base.transform;
		base.Invoke("getAmplifyObj", 0.5f);
		float value = UnityEngine.Random.value;
		base.InvokeRepeating("checkPlayerDist", value, 0.5f);
	}

	private void checkPlayerDist()
	{
		if (!Scene.SceneTracker)
		{
			return;
		}
		this.allPlayers = new List<GameObject>(Scene.SceneTracker.allPlayers);
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		if (this.allPlayers.Count > 1)
		{
			this.allPlayers.Sort((GameObject c1, GameObject c2) => (this.thisTr.position - c1.transform.position).sqrMagnitude.CompareTo((this.thisTr.position - c2.transform.position).sqrMagnitude));
		}
		if (this.allPlayers.Count == 0)
		{
			return;
		}
		if (this.allPlayers[0] && this.allPlayers[0] != null)
		{
			this.playerTr = this.allPlayers[0].transform;
		}
		this.playerDist = Vector3.Distance(this.thisTr.position, this.playerTr.position);
		if (this.rb == null)
		{
			this.rb = base.transform.GetComponent<Rigidbody>();
		}
		if (this.rb)
		{
			this.vel = this.rb.velocity.magnitude;
		}
		if (base.transform.parent != null)
		{
			this.enableAmplifyMotion();
			this.doAmplify = true;
			return;
		}
		if (this.playerDist > this.limitDistance)
		{
			this.disableAmplifyMotion();
		}
		else if (this.playerDist < this.limitDistance && this.vel > 0.2f)
		{
			this.enableAmplifyMotion();
		}
		else
		{
			this.disableAmplifyMotion();
		}
	}

	private void getAmplifyObj()
	{
		if (this.amplifyBase.Length == 0)
		{
			this.amplifyBase = base.transform.GetComponentsInChildren<AmplifyMotionObjectBase>();
			this.doAmplify = true;
		}
		this.disableAmplifyMotion();
	}

	private void disableAmplifyMotion()
	{
		if (this.amplifyBase != null)
		{
			for (int i = 0; i < this.amplifyBase.Length; i++)
			{
				this.amplifyBase[i].enabled = false;
			}
		}
	}

	private void enableAmplifyMotion()
	{
		if (this.amplifyBase != null)
		{
			for (int i = 0; i < this.amplifyBase.Length; i++)
			{
				this.amplifyBase[i].enabled = true;
			}
		}
	}
}
