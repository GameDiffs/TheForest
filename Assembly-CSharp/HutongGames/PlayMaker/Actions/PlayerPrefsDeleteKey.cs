using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs"), HutongGames.PlayMaker.Tooltip("Removes key and its corresponding value from the preferences.")]
	public class PlayerPrefsDeleteKey : FsmStateAction
	{
		public FsmString key;

		public override void Reset()
		{
			this.key = string.Empty;
		}

		public override void OnEnter()
		{
			if (!this.key.IsNone && !this.key.Value.Equals(string.Empty))
			{
				PlayerPrefs.DeleteKey(this.key.Value);
			}
			base.Finish();
		}
	}
}
