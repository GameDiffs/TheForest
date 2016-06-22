using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class TreePush : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $StopPush$742 : GenericGenerator<WaitForSeconds>
	{
		internal TreePush $self_$744;

		public $StopPush$742(TreePush self_)
		{
			this.$self_$744 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new TreePush.$StopPush$742.$(this.$self_$744);
		}
	}

	private bool Yes;

	public TreePush()
	{
		this.Yes = true;
	}

	public override void Awake()
	{
		this.StartCoroutine_Auto(this.StopPush());
	}

	public override void FixedUpdate()
	{
		if (this.Yes)
		{
			this.GetComponent<Rigidbody>().AddForce((float)UnityEngine.Random.Range(1, 5), (float)UnityEngine.Random.Range(1, 5), (float)UnityEngine.Random.Range(1, 5));
		}
	}

	public override IEnumerator StopPush()
	{
		return new TreePush.$StopPush$742(this).GetEnumerator();
	}

	public override void Main()
	{
	}
}
