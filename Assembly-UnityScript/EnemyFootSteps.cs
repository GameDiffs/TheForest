using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class EnemyFootSteps : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $PlayFootStepsRun$730 : GenericGenerator<WaitForSeconds>
	{
		internal EnemyFootSteps $self_$732;

		public $PlayFootStepsRun$730(EnemyFootSteps self_)
		{
			this.$self_$732 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new EnemyFootSteps.$PlayFootStepsRun$730.$(this.$self_$732);
		}
	}

	public GameObject[] RunSfx;

	private GameObject CurrentFootStep;

	private bool PlayingFootStep;

	private bool Running;

	public override void Update()
	{
		if (this.Running && !this.PlayingFootStep)
		{
			this.StartCoroutine_Auto(this.PlayFootStepsRun());
		}
	}

	public override IEnumerator PlayFootStepsRun()
	{
		return new EnemyFootSteps.$PlayFootStepsRun$730(this).GetEnumerator();
	}

	public override void SoundRun()
	{
		this.Running = true;
	}

	public override void StopSoundRun()
	{
		this.Running = false;
	}

	public override void Main()
	{
	}
}
