using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class DeadTree : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $Force$724 : GenericGenerator<WaitForSeconds>
	{
		internal DeadTree $self_$726;

		public $Force$724(DeadTree self_)
		{
			this.$self_$726 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new DeadTree.$Force$724.$(this.$self_$726);
		}
	}

	public Transform Stump;

	public bool PushMe;

	public DeadTree()
	{
		this.PushMe = true;
	}

	public override void Awake()
	{
		this.Stump.parent = this.transform.parent;
		this.StartCoroutine_Auto(this.Force());
	}

	public override void Update()
	{
		if (this.PushMe)
		{
			this.GetComponent<Rigidbody>().AddForce((float)8, (float)0, (float)0);
		}
	}

	public override IEnumerator Force()
	{
		return new DeadTree.$Force$724(this).GetEnumerator();
	}

	public override void Main()
	{
	}
}
