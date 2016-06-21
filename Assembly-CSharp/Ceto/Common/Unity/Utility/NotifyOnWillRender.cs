using System;

namespace Ceto.Common.Unity.Utility
{
	public class NotifyOnWillRender : NotifyOnEvent
	{
		private void OnWillRenderObject()
		{
			base.OnEvent();
		}
	}
}
