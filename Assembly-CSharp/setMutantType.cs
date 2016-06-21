using System;
using UnityEngine;

public class setMutantType : MonoBehaviour
{
	public enum MutantType
	{
		FemaleSkinny,
		MaleSkinny,
		Leader,
		MaleSkinnyLeader,
		Fireman,
		Pale,
		PaleLeader
	}

	public setMutantType.MutantType Type;

	private void OnEnable()
	{
		if (this.Type == setMutantType.MutantType.MaleSkinnyLeader)
		{
			base.SendMessage("setSkinnyLeader");
			base.SendMessage("setMaleSkinny");
		}
		else
		{
			base.SendMessage("set" + this.Type.ToString());
		}
	}
}
