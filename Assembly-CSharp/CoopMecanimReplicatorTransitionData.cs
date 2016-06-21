using System;
using System.Collections.Generic;
using UnityEngine;

public class CoopMecanimReplicatorTransitionData : ScriptableObject
{
	[Serializable]
	public class TransitionData
	{
		public int HashFrom;

		public int HashTo;

		public float Duration;
	}

	[HideInInspector]
	public Dictionary<int, Dictionary<int, float>> Lookup;

	[SerializeField]
	public CoopMecanimReplicatorTransitionData.TransitionData[] Data;

	public void Init()
	{
		this.Lookup = new Dictionary<int, Dictionary<int, float>>();
		CoopMecanimReplicatorTransitionData.TransitionData[] data = this.Data;
		for (int i = 0; i < data.Length; i++)
		{
			CoopMecanimReplicatorTransitionData.TransitionData transitionData = data[i];
			Dictionary<int, float> dictionary;
			if (!this.Lookup.TryGetValue(transitionData.HashFrom, out dictionary))
			{
				this.Lookup.Add(transitionData.HashFrom, dictionary = new Dictionary<int, float>());
			}
			dictionary.Add(transitionData.HashTo, transitionData.Duration);
		}
	}
}
