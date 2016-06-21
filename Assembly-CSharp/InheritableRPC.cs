using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UniLinq;
using UnityEngine;

[AddComponentMenu("System/Inheritable RPC Handler")]
public class InheritableRPC : MonoBehaviour
{
	public class CachedRoutine
	{
		public MethodInfo routine;

		public MonoBehaviour behaviour;
	}

	private Dictionary<string, List<InheritableRPC.CachedRoutine>> cache = new Dictionary<string, List<InheritableRPC.CachedRoutine>>();

	[RPC]
	private void PerformRPCCall(string routineName, string parameters)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(parameters)))
		{
			object[] parameters2 = (object[])binaryFormatter.Deserialize(memoryStream);
			if (!this.cache.ContainsKey(routineName))
			{
				this.cache[routineName] = (from m in base.GetComponents<MonoBehaviour>()
				select new InheritableRPC.CachedRoutine
				{
					routine = m.GetType().GetMethod(routineName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
					behaviour = m
				} into r
				where r.routine != null && r.routine.IsDefined(typeof(RPC), true)
				select r).ToList<InheritableRPC.CachedRoutine>();
			}
			foreach (InheritableRPC.CachedRoutine current in this.cache[routineName])
			{
				current.routine.Invoke(current.behaviour, parameters2);
			}
		}
	}
}
