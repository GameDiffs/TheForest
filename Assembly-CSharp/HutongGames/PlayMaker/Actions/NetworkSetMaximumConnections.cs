using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Set the maximum amount of connections/players allowed.\n\nThis cannot be set higher than the connection count given in Launch Server.\n\nSetting it to 0 means no new connections can be made but the existing ones stay connected.\n\nSetting it to -1 means the maximum connections count is set to the same number of current open connections. In that case, if a players drops then the slot is still open for him.")]
	public class NetworkSetMaximumConnections : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("The maximum amount of connections/players allowed.")]
		public FsmInt maximumConnections;

		public override void Reset()
		{
			this.maximumConnections = 32;
		}

		public override void OnEnter()
		{
			if (this.maximumConnections.Value < -1)
			{
				this.LogWarning("Network Maximum connections can not be less than -1");
				this.maximumConnections.Value = -1;
			}
			Network.maxConnections = this.maximumConnections.Value;
			base.Finish();
		}

		public override string ErrorCheck()
		{
			if (this.maximumConnections.Value < -1)
			{
				return "Network Maximum connections can not be less than -1";
			}
			return string.Empty;
		}
	}
}
