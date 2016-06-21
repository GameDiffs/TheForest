using BoltInternal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UdpKit;
using UnityEngine;

public static class BoltLauncher
{
	private static UdpPlatform UserAssignedPlatform;

	public static void StartSinglePlayer()
	{
		BoltLauncher.StartSinglePlayer(BoltRuntimeSettings.instance.GetConfigCopy());
	}

	public static void StartSinglePlayer(BoltConfig config)
	{
		BoltLauncher.SetUdpPlatform(new NullPlatform());
		BoltLauncher.Initialize(BoltNetworkModes.Host, UdpEndPoint.Any, config);
	}

	public static void StartServer()
	{
		BoltLauncher.StartServer(UdpEndPoint.Any);
	}

	public static void StartServer(int port)
	{
		if (port >= 0 && port <= 65535)
		{
			BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)port));
			return;
		}
		throw new ArgumentOutOfRangeException(string.Format("'port' must be >= 0 and <= {0}", 65535));
	}

	public static void StartServer(BoltConfig config)
	{
		BoltLauncher.StartServer(UdpEndPoint.Any, config);
	}

	public static void StartServer(UdpEndPoint endpoint)
	{
		BoltLauncher.StartServer(endpoint, BoltRuntimeSettings.instance.GetConfigCopy());
	}

	public static void StartServer(UdpEndPoint endpoint, BoltConfig config)
	{
		BoltLauncher.Initialize(BoltNetworkModes.Host, endpoint, config);
	}

	public static void StartClient()
	{
		BoltLauncher.StartClient(UdpEndPoint.Any);
	}

	public static void StartClient(BoltConfig config)
	{
		BoltLauncher.StartClient(UdpEndPoint.Any, config);
	}

	public static void StartClient(UdpEndPoint endpoint)
	{
		BoltLauncher.StartClient(endpoint, BoltRuntimeSettings.instance.GetConfigCopy());
	}

	public static void StartClient(UdpEndPoint endpoint, BoltConfig config)
	{
		BoltLauncher.Initialize(BoltNetworkModes.Client, endpoint, config);
	}

	public static void StartClient(int port)
	{
		if (port >= 0 && port <= 65535)
		{
			BoltLauncher.StartClient(new UdpEndPoint(UdpIPv4Address.Any, (ushort)port));
			return;
		}
		throw new ArgumentOutOfRangeException(string.Format("'port' must be >= 0 and <= {0}", 65535));
	}

	public static void Shutdown()
	{
		BoltNetworkInternal.__Shutdown();
	}

	private static void Initialize(BoltNetworkModes modes, UdpEndPoint endpoint, BoltConfig config)
	{
		BoltNetworkInternal.DebugDrawer = new UnityDebugDrawer();
		BoltNetworkInternal.UsingUnityPro = true;
		BoltNetworkInternal.GetSceneName = new Func<int, string>(BoltLauncher.GetSceneName);
		BoltNetworkInternal.GetSceneIndex = new Func<string, int>(BoltLauncher.GetSceneIndex);
		BoltNetworkInternal.GetGlobalBehaviourTypes = new Func<List<STuple<BoltGlobalBehaviourAttribute, Type>>>(BoltLauncher.GetGlobalBehaviourTypes);
		BoltNetworkInternal.EnvironmentSetup = new Action(BoltNetworkInternal_User.EnvironmentSetup);
		BoltNetworkInternal.EnvironmentReset = new Action(BoltNetworkInternal_User.EnvironmentReset);
		BoltNetworkInternal.__Initialize(modes, endpoint, config, BoltLauncher.CreateUdpPlatform(), null);
	}

	private static int GetSceneIndex(string name)
	{
		return BoltScenes_Internal.GetSceneIndex(name);
	}

	private static string GetSceneName(int index)
	{
		return BoltScenes_Internal.GetSceneName(index);
	}

	public static List<STuple<BoltGlobalBehaviourAttribute, Type>> GetGlobalBehaviourTypes()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		List<STuple<BoltGlobalBehaviourAttribute, Type>> list = new List<STuple<BoltGlobalBehaviourAttribute, Type>>();
		try
		{
			Type[] types = executingAssembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (typeof(MonoBehaviour).IsAssignableFrom(type))
				{
					BoltGlobalBehaviourAttribute[] array = (BoltGlobalBehaviourAttribute[])type.GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), false);
					if (array.Length == 1)
					{
						list.Add(new STuple<BoltGlobalBehaviourAttribute, Type>(array[0], type));
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	public static void SetUdpPlatform(UdpPlatform platform)
	{
		BoltLauncher.UserAssignedPlatform = platform;
	}

	public static UdpPlatform CreateUdpPlatform()
	{
		if (BoltLauncher.UserAssignedPlatform != null)
		{
			return BoltLauncher.UserAssignedPlatform;
		}
		return new DotNetPlatform();
	}
}
