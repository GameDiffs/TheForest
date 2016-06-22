using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class DoorGeneric : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $Stop$727 : GenericGenerator<WaitForSeconds>
	{
		internal DoorGeneric $self_$729;

		public $Stop$727(DoorGeneric self_)
		{
			this.$self_$729 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new DoorGeneric.$Stop$727.$(this.$self_$729);
		}
	}

	public Transform Left;

	public Transform Right;

	public bool ShouldOpen;

	public int Speed;

	public DoorGeneric()
	{
		this.Speed = 1;
	}

	public override void OpenDoor()
	{
		this.ShouldOpen = true;
		this.StartCoroutine_Auto(this.Stop());
	}

	public override void Update()
	{
		if (this.ShouldOpen)
		{
			this.Left.Translate(Vector3.right * (float)this.Speed * Time.deltaTime);
			this.Right.Translate(Vector3.left * (float)this.Speed * Time.deltaTime);
		}
	}

	public override IEnumerator Stop()
	{
		return new DoorGeneric.$Stop$727(this).GetEnumerator();
	}

	public override void Main()
	{
	}
}
