using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList"), HutongGames.PlayMaker.Tooltip("Store all resolutions")]
	public class ArrayListGetScreenResolutions : ArrayListActions
	{
		[ActionSection("Set up"), CheckForComponent(typeof(PlayMakerArrayListProxy)), RequiredField, HutongGames.PlayMaker.Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.getResolutions();
			}
			base.Finish();
		}

		public void getResolutions()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Clear();
			Resolution[] resolutions = Screen.resolutions;
			Resolution[] array = resolutions;
			for (int i = 0; i < array.Length; i++)
			{
				Resolution resolution = array[i];
				this.proxy.arrayList.Add(new Vector3((float)resolution.width, (float)resolution.height, (float)resolution.refreshRate));
			}
		}
	}
}
