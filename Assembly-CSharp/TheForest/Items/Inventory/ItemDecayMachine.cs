using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic]
	public class ItemDecayMachine : MonoBehaviour
	{
		[Serializable]
		private class ItemDecayCommand
		{
			public DecayingInventoryItemView _diiv;

			public float _decayTime;
		}

		private List<ItemDecayMachine.ItemDecayCommand> _commands = new List<ItemDecayMachine.ItemDecayCommand>();

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (this._commands[0]._decayTime < Time.time)
			{
				ItemDecayMachine.ItemDecayCommand itemDecayCommand = this._commands[0];
				this._commands.RemoveAt(0);
				itemDecayCommand._diiv.Decay();
				if (this._commands.Count == 0)
				{
					base.enabled = false;
				}
			}
		}

		public void NewDecayCommand(DecayingInventoryItemView diiv, float decayDuration)
		{
			int num = 0;
			float decayTime = Time.time + decayDuration;
			this.CancelCommandFor(diiv);
			if (this._commands.Count > 0)
			{
				num = this._commands.IndexOf(this._commands.FirstOrDefault((ItemDecayMachine.ItemDecayCommand c) => c._decayTime > decayTime));
				if (num < 0)
				{
					num = this._commands.Count;
				}
			}
			this._commands.Insert(num, new ItemDecayMachine.ItemDecayCommand
			{
				_diiv = diiv,
				_decayTime = decayTime
			});
			base.enabled = true;
		}

		public void CancelCommandFor(DecayingInventoryItemView diiv)
		{
			int num = this._commands.IndexOf(this._commands.FirstOrDefault((ItemDecayMachine.ItemDecayCommand c) => c._diiv.Equals(diiv)));
			if (num >= 0)
			{
				this._commands.RemoveAt(num);
			}
			if (this._commands.Count == 0)
			{
				base.enabled = false;
			}
		}
	}
}
