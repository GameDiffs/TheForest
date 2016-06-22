using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class KillAftertime : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $Kill$733 : GenericGenerator<WaitForSeconds>
	{
		internal KillAftertime $self_$735;

		public $Kill$733(KillAftertime self_)
		{
			this.$self_$735 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new KillAftertime.$Kill$733.$(this.$self_$735);
		}
	}

	public float Amount;

	public override void Awake()
	{
		Debug.Log("TEST");
		this.StartCoroutine_Auto(this.Kill());
	}

	public override IEnumerator Kill()
	{
		return new KillAftertime.$Kill$733(this).GetEnumerator();
	}

	public override void Main()
	{
	}
}
