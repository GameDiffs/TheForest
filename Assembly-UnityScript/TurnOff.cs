using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class TurnOff : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $TurnOff$745 : GenericGenerator<WaitForSeconds>
	{
		internal TurnOff $self_$747;

		public $TurnOff$745(TurnOff self_)
		{
			this.$self_$747 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new TurnOff.$TurnOff$745.$(this.$self_$747);
		}
	}

	public int Wait;

	public TurnOff()
	{
		this.Wait = 2;
	}

	public override void Start()
	{
		this.StartCoroutine_Auto(this.TurnOff());
	}

	public override IEnumerator TurnOff()
	{
		return new TurnOff.$TurnOff$745(this).GetEnumerator();
	}

	public override void Main()
	{
	}
}
