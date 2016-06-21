using System;
using UnityEngine;

public class playerMeshSwitcher : MonoBehaviour
{
	private netPlayerVis pVis;

	private Transform tr;

	private float dist;

	public Mesh Lod0;

	public Mesh Lod1;

	public Mesh Lod2;

	public GameObject[] lodJoints;

	public bool jointsUseLod2Distance;

	private SkinnedMeshRenderer skin;

	private playerScriptSetup setup;

	public float lod1Distance = 15f;

	public float lod2Distance = 35f;

	private int frameOffset;

	private int frameInterval = 10;

	private void Start()
	{
		this.pVis = base.transform.root.GetComponent<netPlayerVis>();
		this.tr = base.transform;
		this.skin = base.transform.GetComponent<SkinnedMeshRenderer>();
		this.setup = base.transform.root.GetComponentInChildren<playerScriptSetup>();
	}

	private void Update()
	{
		if ((Time.frameCount + this.frameOffset) % this.frameInterval != 0)
		{
			return;
		}
		this.updateVis();
	}

	private void updateVis()
	{
		if (!this.pVis)
		{
			return;
		}
		if (!this.skin.enabled)
		{
			return;
		}
		if (this.pVis.localplayerDist < this.lod1Distance)
		{
			this.skin.sharedMesh = this.Lod0;
			this.enableLodJoints();
		}
		else if (this.pVis.localplayerDist < this.lod2Distance)
		{
			this.skin.sharedMesh = this.Lod1;
			if (!this.jointsUseLod2Distance)
			{
				this.disableLodJoints();
			}
		}
		else
		{
			this.skin.sharedMesh = this.Lod2;
			this.disableLodJoints();
		}
	}

	private void enableLodJoints()
	{
		if (this.lodJoints.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.lodJoints.Length; i++)
		{
			this.lodJoints[i].SetActive(true);
		}
	}

	private void disableLodJoints()
	{
		if (this.lodJoints.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.lodJoints.Length; i++)
		{
			this.lodJoints[i].SetActive(false);
		}
	}
}
