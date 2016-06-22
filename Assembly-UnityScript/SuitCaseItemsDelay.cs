using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class SuitCaseItemsDelay : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $Delay$739 : GenericGenerator<WaitForSeconds>
	{
		internal SuitCaseItemsDelay $self_$741;

		public $Delay$739(SuitCaseItemsDelay self_)
		{
			this.$self_$741 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new SuitCaseItemsDelay.$Delay$739.$(this.$self_$741);
		}
	}

	public GameObject MyTrigger;

	public float DelayAmount;

	public SuitCaseItemsDelay()
	{
		this.DelayAmount = 1f;
	}

	public override IEnumerator Delay()
	{
		return new SuitCaseItemsDelay.$Delay$739(this).GetEnumerator();
	}

	public override void Main()
	{
	}
}
