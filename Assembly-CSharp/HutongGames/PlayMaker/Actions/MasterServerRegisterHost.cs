using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Register this server on the master server.\n\nIf the master server address information has not been changed the default Unity master server will be used.")]
	public class MasterServerRegisterHost : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The unique game type name.")]
		public FsmString gameTypeName;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The game name.")]
		public FsmString gameName;

		[HutongGames.PlayMaker.Tooltip("Optional comment")]
		public FsmString comment;

		public override void Reset()
		{
			this.gameTypeName = null;
			this.gameName = null;
			this.comment = null;
		}

		public override void OnEnter()
		{
			this.DoMasterServerRegisterHost();
			base.Finish();
		}

		private void DoMasterServerRegisterHost()
		{
			MasterServer.RegisterHost(this.gameTypeName.Value, this.gameName.Value, this.comment.Value);
		}
	}
}
