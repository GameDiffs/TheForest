using System;
using System.IO;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application), HutongGames.PlayMaker.Tooltip("Saves a Screenshot to the users MyPictures folder. TIP: Can be useful for automated testing and debugging.")]
	public class TakeScreenshot : FsmStateAction
	{
		[RequiredField]
		public FsmString filename;

		public bool autoNumber;

		private int screenshotCount;

		public override void Reset()
		{
			this.filename = string.Empty;
			this.autoNumber = false;
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.filename.Value))
			{
				return;
			}
			string text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/";
			string path = text + this.filename.Value + ".png";
			if (this.autoNumber)
			{
				while (File.Exists(path))
				{
					this.screenshotCount++;
					path = string.Concat(new object[]
					{
						text,
						this.filename.Value,
						this.screenshotCount,
						".png"
					});
				}
			}
			Application.CaptureScreenshot(path);
			base.Finish();
		}
	}
}
