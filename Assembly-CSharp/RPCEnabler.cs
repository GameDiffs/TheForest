using System;
using UnityEngine;

public static class RPCEnabler
{
	public static bool Others(this NetworkView networkView, string routineName, params object[] parameters)
	{
		if (!networkView.isMine)
		{
			networkView.RPC(routineName, RPCMode.Others, parameters);
		}
		return !networkView.isMine;
	}

	public static bool Server(this NetworkView networkView, string routineName, params object[] parameters)
	{
		networkView.RPC(routineName, RPCMode.Server, parameters);
		return Network.isServer;
	}
}
