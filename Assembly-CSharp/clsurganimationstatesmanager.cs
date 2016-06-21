using System;
using System.Collections.Generic;
using UnityEngine;

public class clsurganimationstatesmanager : MonoBehaviour
{
	public string vargamrootname;

	[HideInInspector]
	public Vector3 vargamrootoriginallocalposition;

	[HideInInspector]
	public List<string> vargamstatenames;

	[HideInInspector]
	public List<clsanimationstatesnapshot> vargamanimationstates;

	public void metremove(string varpname)
	{
		int num = this.vargamstatenames.IndexOf(varpname);
		if (num > -1)
		{
			this.vargamstatenames.RemoveAt(num);
			this.vargamanimationstates.RemoveAt(num);
		}
	}

	public void metadd(string varpname, clsanimationstatesnapshot varpsnapshot)
	{
		this.metremove(varpname);
		this.vargamstatenames.Add(varpname);
		this.vargamanimationstates.Add(varpsnapshot);
	}
}
