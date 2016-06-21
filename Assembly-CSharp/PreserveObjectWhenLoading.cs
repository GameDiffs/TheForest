using System;
using UnityEngine;

[AddComponentMenu("Components/Storage/Preserve Object When Loading")]
public class PreserveObjectWhenLoading : MonoBehaviour
{
	private void Awake()
	{
		LevelLoader.OnDestroyObject += new LevelLoader.SerializedObjectDelegate(this.HandleLevelLoaderOnDestroyObject);
	}

	private void HandleLevelLoaderOnDestroyObject(GameObject toBeDestroyed, ref bool cancel)
	{
		cancel = true;
	}

	private void OnDestroy()
	{
		LevelLoader.OnDestroyObject -= new LevelLoader.SerializedObjectDelegate(this.HandleLevelLoaderOnDestroyObject);
	}
}
