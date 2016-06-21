using System;
using UnityEngine;

[AddComponentMenu("Storage/Advanced/In Range Item")]
public class InRangeItem : MonoBehaviour
{
	private void Start()
	{
		if (OnlyInRangeManager.Instance != null)
		{
			OnlyInRangeManager.Instance.AddRangedItem(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (OnlyInRangeManager.Instance != null)
		{
			OnlyInRangeManager.Instance.DestroyRangedItem(base.gameObject);
		}
	}
}
