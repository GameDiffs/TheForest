using System;
using UnityEngine;

public class fishPosFix : MonoBehaviour
{
	private Fish fish;

	public Transform refPos;

	private Transform tr;

	private AmplifyMotionObjectBase amb;

	private void Start()
	{
		this.tr = base.transform;
		this.fish = base.transform.parent.GetComponent<Fish>();
	}

	private void Update()
	{
		if (this.fish.spearedBool)
		{
			if (!this.amb)
			{
				this.amb = base.gameObject.AddComponent<AmplifyMotionObjectBase>();
			}
			else
			{
				this.amb.enabled = true;
			}
			this.tr.position = this.refPos.position;
		}
		else if (this.amb)
		{
			this.amb.enabled = false;
		}
	}

	private void LateUpdate()
	{
		if (this.fish.spearedBool)
		{
			this.tr.position = this.refPos.position;
		}
	}
}
