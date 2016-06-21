using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class InheritableRPCExtensions
{
	public class StoredPlayer
	{
		public string ipAddress;

		public string guid;

		public int port;
	}

	public static void RPCEx(this NetworkView view, string routineName, RPCMode mode, params object[] parameters)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, parameters);
			memoryStream.Flush();
			string text = Convert.ToBase64String(memoryStream.GetBuffer());
			view.RPC("PerformRPCCall", mode, new object[]
			{
				routineName,
				text
			});
		}
	}

	public static void RPCEx(this NetworkView view, string routineName, NetworkPlayer player, params object[] parameters)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, parameters);
			memoryStream.Flush();
			string text = Convert.ToBase64String(memoryStream.GetBuffer());
			view.RPC("PerformRPCCall", player, new object[]
			{
				routineName,
				text
			});
		}
	}
}
