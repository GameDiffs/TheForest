using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

public static class PlaneCrashLocations
{
	private class NameComparer : IComparer<string>
	{
		int IComparer<string>.Compare(string x, string y)
		{
			return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
		}
	}

	private static GameObject[] _markers;

	private static GameObject[] _finalPositions;

	public static GameObject[] markers
	{
		get
		{
			PlaneCrashLocations.LoadMarkers();
			return PlaneCrashLocations._markers;
		}
	}

	public static GameObject[] finalPositions
	{
		get
		{
			PlaneCrashLocations.LoadMarkers();
			return PlaneCrashLocations._finalPositions;
		}
	}

	public static int crashSite
	{
		get;
		set;
	}

	public static void FindMarkers()
	{
		PlaneCrashLocations._markers = (from x in GameObject.FindGameObjectsWithTag("crashMarker")
		where x.activeInHierarchy
		select x).OrderBy((GameObject x) => x.name, new PlaneCrashLocations.NameComparer()).ToArray<GameObject>();
		PlaneCrashLocations._finalPositions = (from x in PlaneCrashLocations._markers
		select x.transform.GetChild(0).gameObject).ToArray<GameObject>();
	}

	private static void LoadMarkers()
	{
		if (PlaneCrashLocations._markers == null || PlaneCrashLocations._finalPositions == null)
		{
			PlaneCrashLocations.FindMarkers();
			return;
		}
		for (int i = 0; i < PlaneCrashLocations._markers.Length; i++)
		{
			if (!PlaneCrashLocations._markers[i])
			{
				PlaneCrashLocations.FindMarkers();
				return;
			}
		}
		for (int j = 0; j < PlaneCrashLocations._finalPositions.Length; j++)
		{
			if (!PlaneCrashLocations._finalPositions[j])
			{
				PlaneCrashLocations.FindMarkers();
				return;
			}
		}
	}
}
