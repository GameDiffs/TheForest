using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Plays a full-screen movie on a handheld device. Please consult the Unity docs for Handheld.PlayFullScreenMovie for proper usage.")]
	public class DevicePlayFullScreenMovie : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Note that player will stream movie directly from the iPhone disc, therefore you have to provide movie as a separate files and not as an usual asset.\nYou will have to create a folder named StreamingAssets inside your Unity project (inside your Assets folder). Store your movies inside that folder. Unity will automatically copy contents of that folder into the iPhone application bundle.")]
		public FsmString moviePath;

		[RequiredField, HutongGames.PlayMaker.Tooltip("This action will initiate a transition that fades the screen from your current content to the designated background color of the player. When playback finishes, the player uses another fade effect to transition back to your content.")]
		public FsmColor fadeColor;

		[ActionSection("Current platform is not iOS or Android")]
		public bool RemindMeAtRuntime;

		public override void Reset()
		{
			this.RemindMeAtRuntime = true;
		}

		public override void OnEnter()
		{
			if (this.RemindMeAtRuntime)
			{
				Debug.LogWarning("Current platform is not iOS or Android, DevicePlayFullScreenMovie action only works for iOS and Android");
			}
		}
	}
}
