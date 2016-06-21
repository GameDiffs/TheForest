using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MB3_MeshBakerGrouperCore
{
	public enum ClusterType
	{
		none,
		grid,
		pie
	}

	[Serializable]
	public class ClusterGrouper
	{
		public MB3_MeshBakerGrouperCore.ClusterType clusterType;

		public Vector3 origin;

		public Vector3 cellSize;

		public int pieNumSegments = 4;

		public Vector3 pieAxis = Vector3.up;

		public Dictionary<string, List<Renderer>> FilterIntoGroups(List<GameObject> selection)
		{
			if (this.clusterType == MB3_MeshBakerGrouperCore.ClusterType.none)
			{
				return this.FilterIntoGroupsNone(selection);
			}
			if (this.clusterType == MB3_MeshBakerGrouperCore.ClusterType.grid)
			{
				return this.FilterIntoGroupsGrid(selection);
			}
			if (this.clusterType == MB3_MeshBakerGrouperCore.ClusterType.pie)
			{
				return this.FilterIntoGroupsPie(selection);
			}
			return new Dictionary<string, List<Renderer>>();
		}

		public Dictionary<string, List<Renderer>> FilterIntoGroupsNone(List<GameObject> selection)
		{
			Debug.Log("Filtering into groups none");
			Dictionary<string, List<Renderer>> dictionary = new Dictionary<string, List<Renderer>>();
			List<Renderer> list = new List<Renderer>();
			for (int i = 0; i < selection.Count; i++)
			{
				list.Add(selection[i].GetComponent<Renderer>());
			}
			dictionary.Add("MeshBaker", list);
			return dictionary;
		}

		public Dictionary<string, List<Renderer>> FilterIntoGroupsGrid(List<GameObject> selection)
		{
			Dictionary<string, List<Renderer>> dictionary = new Dictionary<string, List<Renderer>>();
			if (this.cellSize.x <= 0f || this.cellSize.y <= 0f || this.cellSize.z <= 0f)
			{
				Debug.LogError("cellSize x,y,z must all be greater than zero.");
				return dictionary;
			}
			Debug.Log("Collecting renderers in each cell");
			foreach (GameObject current in selection)
			{
				GameObject gameObject = current;
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] is MeshRenderer || componentsInChildren[i] is SkinnedMeshRenderer)
					{
						Vector3 position = componentsInChildren[i].transform.position;
						position.x = Mathf.Floor((position.x - this.origin.x) / this.cellSize.x) * this.cellSize.x;
						position.y = Mathf.Floor((position.y - this.origin.y) / this.cellSize.y) * this.cellSize.y;
						position.z = Mathf.Floor((position.z - this.origin.z) / this.cellSize.z) * this.cellSize.z;
						string key = position.ToString();
						List<Renderer> list;
						if (dictionary.ContainsKey(key))
						{
							list = dictionary[key];
						}
						else
						{
							list = new List<Renderer>();
							dictionary.Add(key, list);
						}
						if (!list.Contains(componentsInChildren[i]))
						{
							list.Add(componentsInChildren[i]);
						}
					}
				}
			}
			return dictionary;
		}

		public Dictionary<string, List<Renderer>> FilterIntoGroupsPie(List<GameObject> selection)
		{
			Dictionary<string, List<Renderer>> dictionary = new Dictionary<string, List<Renderer>>();
			if (this.pieNumSegments == 0)
			{
				Debug.LogError("pieNumSegments must be greater than zero.");
				return dictionary;
			}
			if (this.pieAxis.magnitude <= 1E-06f)
			{
				Debug.LogError("Pie axis must have length greater than zero.");
				return dictionary;
			}
			this.pieAxis.Normalize();
			Quaternion rotation = Quaternion.FromToRotation(this.pieAxis, Vector3.up);
			Debug.Log("Collecting renderers in each cell");
			foreach (GameObject current in selection)
			{
				GameObject gameObject = current;
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] is MeshRenderer || componentsInChildren[i] is SkinnedMeshRenderer)
					{
						Vector3 point = componentsInChildren[i].transform.position - this.origin;
						point.Normalize();
						point = rotation * point;
						float num;
						if (Mathf.Abs(point.x) < 0.0001f && Mathf.Abs(point.z) < 0.0001f)
						{
							num = 0f;
						}
						else
						{
							num = Mathf.Atan2(point.z, point.x) * 57.29578f;
							if (num < 0f)
							{
								num = 360f + num;
							}
						}
						int num2 = Mathf.FloorToInt(num / 360f * (float)this.pieNumSegments);
						string key = "seg_" + num2;
						List<Renderer> list;
						if (dictionary.ContainsKey(key))
						{
							list = dictionary[key];
						}
						else
						{
							list = new List<Renderer>();
							dictionary.Add(key, list);
						}
						if (!list.Contains(componentsInChildren[i]))
						{
							list.Add(componentsInChildren[i]);
						}
					}
				}
			}
			return dictionary;
		}
	}

	public MB3_MeshBakerGrouperCore.ClusterGrouper clusterGrouper;

	public bool clusterOnLMIndex;

	public void DoClustering(MB3_TextureBaker tb)
	{
		if (this.clusterGrouper == null)
		{
			Debug.LogError("Cluster Grouper was null.");
			return;
		}
		Dictionary<string, List<Renderer>> dictionary = this.clusterGrouper.FilterIntoGroups(tb.GetObjectsToCombine());
		Debug.Log("Found " + dictionary.Count + " cells with Renderers. Creating bakers.");
		if (this.clusterOnLMIndex)
		{
			Dictionary<string, List<Renderer>> dictionary2 = new Dictionary<string, List<Renderer>>();
			foreach (string current in dictionary.Keys)
			{
				List<Renderer> gaws = dictionary[current];
				Dictionary<int, List<Renderer>> dictionary3 = this.GroupByLightmapIndex(gaws);
				foreach (int current2 in dictionary3.Keys)
				{
					string key = current + "-LM-" + current2;
					dictionary2.Add(key, dictionary3[current2]);
				}
			}
			dictionary = dictionary2;
		}
		foreach (string current3 in dictionary.Keys)
		{
			List<Renderer> gaws2 = dictionary[current3];
			this.AddMeshBaker(tb, current3, gaws2);
		}
	}

	private Dictionary<int, List<Renderer>> GroupByLightmapIndex(List<Renderer> gaws)
	{
		Dictionary<int, List<Renderer>> dictionary = new Dictionary<int, List<Renderer>>();
		for (int i = 0; i < gaws.Count; i++)
		{
			List<Renderer> list;
			if (dictionary.ContainsKey(gaws[i].lightmapIndex))
			{
				list = dictionary[gaws[i].lightmapIndex];
			}
			else
			{
				list = new List<Renderer>();
				dictionary.Add(gaws[i].lightmapIndex, list);
			}
			list.Add(gaws[i]);
		}
		return dictionary;
	}

	private void AddMeshBaker(MB3_TextureBaker tb, string key, List<Renderer> gaws)
	{
		int num = 0;
		for (int i = 0; i < gaws.Count; i++)
		{
			Mesh mesh = MB_Utility.GetMesh(gaws[i].gameObject);
			if (mesh != null)
			{
				num += mesh.vertexCount;
			}
		}
		GameObject gameObject = new GameObject("MeshBaker-" + key);
		gameObject.transform.position = Vector3.zero;
		MB3_MeshBakerCommon mB3_MeshBakerCommon;
		if (num >= 65535)
		{
			mB3_MeshBakerCommon = gameObject.AddComponent<MB3_MultiMeshBaker>();
			mB3_MeshBakerCommon.useObjsToMeshFromTexBaker = false;
		}
		else
		{
			mB3_MeshBakerCommon = gameObject.AddComponent<MB3_MeshBaker>();
			mB3_MeshBakerCommon.useObjsToMeshFromTexBaker = false;
		}
		mB3_MeshBakerCommon.textureBakeResults = tb.textureBakeResults;
		mB3_MeshBakerCommon.transform.parent = tb.transform;
		for (int j = 0; j < gaws.Count; j++)
		{
			mB3_MeshBakerCommon.GetObjectsToCombine().Add(gaws[j].gameObject);
		}
	}
}
