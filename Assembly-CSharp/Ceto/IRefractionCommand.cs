using System;

namespace Ceto
{
	public interface IRefractionCommand
	{
		bool DisableCopyDepthCmd
		{
			get;
			set;
		}

		bool DisableNormalFadeCmd
		{
			get;
			set;
		}

		void ClearCommands();

		void UpdateCommands();
	}
}
