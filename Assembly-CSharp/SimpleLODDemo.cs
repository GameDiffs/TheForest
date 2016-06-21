using System;
using UnityEngine;

public class SimpleLODDemo : MonoBehaviour
{
	public GameObject[] scenes;

	private string[] sceneStrings = new string[]
	{
		"Original",
		"Submeshes merged per material",
		"LOD levels generated"
	};

	private int currentScene;

	private int lodSetting;

	private void Start()
	{
		QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1, true);
		for (int i = 1; i < this.scenes.Length; i++)
		{
			this.scenes[i].SetActive(false);
		}
		this.SetScene(0);
	}

	private void SetScene(int aScene)
	{
		this.scenes[this.currentScene].SetActive(false);
		this.scenes[aScene].SetActive(true);
		this.currentScene = aScene;
		Camera.main.gameObject.GetComponent<SimpleLODDemoCamera>().SetCurrentScene(this.scenes[this.currentScene]);
	}

	private void SetLOD(int aLod)
	{
		this.lodSetting = aLod;
		aLod--;
		LODSwitcher[] componentsInChildren = this.scenes[this.currentScene].GetComponentsInChildren<LODSwitcher>();
		LODSwitcher[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			LODSwitcher lODSwitcher = array[i];
			if (aLod < 0)
			{
				lODSwitcher.ReleaseFixedLODLevel();
			}
			else
			{
				lODSwitcher.SetFixedLODLevel(aLod);
			}
		}
	}

	private void OnGUI()
	{
		GUI.skin.label.normal.textColor = Color.black;
		GUI.Label(new Rect(2f, 70f, 200f, 100f), "Switch scene to:");
		int num = GUI.SelectionGrid(new Rect(2f, 90f, 250f, 80f), this.currentScene, this.sceneStrings, 1);
		if (num != this.currentScene)
		{
			this.SetScene(num);
		}
		if (this.currentScene == 2)
		{
			GUI.Label(new Rect((float)(Screen.width - 102), 2f, 100f, 24f), "Set LOD to:");
			int num2 = GUI.SelectionGrid(new Rect((float)(Screen.width - 102), 24f, 100f, 110f), this.lodSetting, new string[]
			{
				"Automatic",
				"LOD 0",
				"LOD 1",
				"LOD 2",
				"LOD 3"
			}, 1);
			if (num2 != this.lodSetting)
			{
				this.SetLOD(num2);
			}
		}
	}
}
