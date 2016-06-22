using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class CreatureRagdoll : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $TurnOffSound$721 : GenericGenerator<WaitForSeconds>
	{
		internal CreatureRagdoll $self_$723;

		public $TurnOffSound$721(CreatureRagdoll self_)
		{
			this.$self_$723 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new CreatureRagdoll.$TurnOffSound$721.$(this.$self_$723);
		}
	}

	public GameObject Sfx;

	public override void Awake()
	{
		this.StartCoroutine_Auto(this.TurnOffSound());
		this.GetComponent<Animation>().wrapMode = WrapMode.Once;
	}

	public override IEnumerator TurnOffSound()
	{
		return new CreatureRagdoll.$TurnOffSound$721(this).GetEnumerator();
	}

	public override void Main()
	{
	}
}
