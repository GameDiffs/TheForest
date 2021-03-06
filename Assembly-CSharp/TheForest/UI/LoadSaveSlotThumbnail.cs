using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TheForest.UI
{
	public class LoadSaveSlotThumbnail : MonoBehaviour
	{
		public UITexture _texture;

		public TitleScreen.GameSetup.Slots _slot;

		[HideInInspector]
		public int _slotNum;

		private void OnEnable()
		{
			this._slotNum = (int)this._slot;
			base.StartCoroutine(this.LoadImageRoutine());
		}

		[DebuggerHidden]
		private IEnumerator LoadImageRoutine()
		{
			LoadSaveSlotThumbnail.<LoadImageRoutine>c__Iterator1BD <LoadImageRoutine>c__Iterator1BD = new LoadSaveSlotThumbnail.<LoadImageRoutine>c__Iterator1BD();
			<LoadImageRoutine>c__Iterator1BD.<>f__this = this;
			return <LoadImageRoutine>c__Iterator1BD;
		}
	}
}
