using Bolt;
using System;

public class CoopServerInfo : EntityBehaviour<ICoopServerInfo>
{
	public static CoopServerInfo Instance;

	public override void Attached()
	{
		CoopServerInfo.Instance = this;
	}

	public override void Detached()
	{
		CoopServerInfo.Instance = null;
	}
}
