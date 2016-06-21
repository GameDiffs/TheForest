using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Core
{
	public class ItemAnimatorHashHelper
	{
		private int[] _hashes;

		public ItemAnimatorHashHelper()
		{
			string[] names = Enum.GetNames(typeof(Item.AnimatorVariables));
			this._hashes = new int[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				this._hashes[i] = Animator.StringToHash(names[i]);
			}
		}

		public void ApplyAnimVars(Item item, bool active)
		{
			for (int i = 0; i < item._equipedAnimVars.Length; i++)
			{
				LocalPlayer.Animator.SetBool(this._hashes[(int)item._equipedAnimVars[i]], active);
			}
		}
	}
}
