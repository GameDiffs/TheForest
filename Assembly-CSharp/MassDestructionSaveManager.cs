using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

[RequireComponent(typeof(StoreInformation))]
public class MassDestructionSaveManager : MonoBehaviour
{
	[DoNotSerialize]
	public GameObject StumpPrefab;

	[DoNotSerialize]
	public bool MatchTreeScale = true;

	[HideInInspector]
	public int[] CutDownTreeIds;

	[DoNotSerialize]
	public int TreeCount;

	private void Awake()
	{
		CoopPlayerCallbacks.ClearTrees();
		base.enabled = false;
	}

	private void OnSerializing()
	{
		LOD_Trees[] source = UnityEngine.Object.FindObjectsOfType<LOD_Trees>();
		List<int> list = (from t in source
		where !t.enabled && t.CurrentView == null
		select t.GetComponent<CoopTreeId>() into cti
		where cti != null
		select cti.Id).ToList<int>();
		IEnumerable<int> enumerable = from t in source
		select t.GetComponent<CoopTreeId>() into cti
		where cti != null
		select cti.Id into t
		orderby t
		select t;
		int i = 0;
		foreach (int current in enumerable)
		{
			while (i < current)
			{
				list.Add(-i);
				i++;
			}
			i = current + 1;
		}
		while (i < this.TreeCount)
		{
			list.Add(-i);
			i++;
		}
		this.CutDownTreeIds = list.ToArray();
		Debug.Log("Saving " + this.CutDownTreeIds.Length + " trees that were cut down");
	}

	private void OnDeserialized()
	{
		base.enabled = true;
	}

	private void Update()
	{
		base.enabled = false;
		Debug.Log("Turning off " + this.CutDownTreeIds.Length + " trees that were cut down");
		Dictionary<int, CoopTreeId> dictionary = UnityEngine.Object.FindObjectsOfType<CoopTreeId>().ToDictionary((CoopTreeId t) => t.Id);
		for (int i = 0; i < this.CutDownTreeIds.Length; i++)
		{
			CoopTreeId coopTreeId;
			if (dictionary.TryGetValue(Mathf.Abs(this.CutDownTreeIds[i]), out coopTreeId))
			{
				if (this.CutDownTreeIds[i] >= 0)
				{
					LOD_Trees component = coopTreeId.GetComponent<LOD_Trees>();
					if (component)
					{
						if (component.CurrentLodTransform)
						{
							UnityEngine.Object.Destroy(component.CurrentLodTransform.gameObject);
						}
						if (this.StumpPrefab && component.transform.localScale.x >= 1f)
						{
							Transform transform = component.transform;
							GameObject original = (!component.StumpPrefab) ? this.StumpPrefab : component.StumpPrefab;
							GameObject gameObject = UnityEngine.Object.Instantiate(original, transform.position, transform.rotation) as GameObject;
							if (this.MatchTreeScale)
							{
								gameObject.transform.localScale = component.High.transform.localScale;
							}
							gameObject.transform.parent = coopTreeId.transform;
						}
						TreeHealth.OnTreeCutDown.Invoke(component.transform.position);
						component.enabled = false;
					}
				}
				else
				{
					if (coopTreeId)
					{
						UnityEngine.Object.Instantiate(Resources.Load("stumpGridBlocker"), coopTreeId.transform.position, coopTreeId.transform.rotation);
					}
					UnityEngine.Object.Destroy(coopTreeId.GetComponent<LOD_Base>());
					if (BoltNetwork.isServer)
					{
						coopTreeId.Goto_Removed();
					}
				}
			}
		}
	}
}
