using System;
using UnityEngine;

public class PlatformSpecificContent : MonoBehaviour
{
	private enum BuildTargetGroup
	{
		Standalone,
		Mobile
	}

	[SerializeField]
	private PlatformSpecificContent.BuildTargetGroup showOnlyOn;

	[SerializeField]
	private GameObject[] content = new GameObject[0];

	[SerializeField]
	private bool childrenOfThisObject;

	private void OnEnable()
	{
		this.CheckEnableContent();
	}

	private void CheckEnableContent()
	{
		if (this.showOnlyOn == PlatformSpecificContent.BuildTargetGroup.Mobile)
		{
			this.EnableContent(false);
		}
		else
		{
			this.EnableContent(true);
		}
	}

	private void EnableContent(bool enabled)
	{
		if (this.content.Length > 0)
		{
			GameObject[] array = this.content;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i];
				if (gameObject != null)
				{
					gameObject.SetActive(enabled);
				}
			}
		}
		if (this.childrenOfThisObject)
		{
			foreach (Transform transform in base.transform)
			{
				transform.gameObject.SetActive(enabled);
			}
		}
	}
}
