using Bolt;
using System;
using System.Collections.Generic;

public static class CoopAdminCommand
{
	public static void Send(string command, string data)
	{
		if (CoopPeerStarter.Dedicated)
		{
			AdminCommand adminCommand = AdminCommand.Create(GlobalTargets.OnlyServer);
			adminCommand.Command = command;
			adminCommand.Data = data;
		}
	}

	public static void Recv(string command, string data)
	{
		if (CoopPeerStarter.DedicatedHost && command != null)
		{
			if (CoopAdminCommand.<>f__switch$map7 == null)
			{
				CoopAdminCommand.<>f__switch$map7 = new Dictionary<string, int>(2)
				{
					{
						"save",
						0
					},
					{
						"restart",
						1
					}
				};
			}
			int num;
			if (CoopAdminCommand.<>f__switch$map7.TryGetValue(command, out num))
			{
				if (num != 0)
				{
					if (num != 1)
					{
					}
				}
			}
		}
	}
}
