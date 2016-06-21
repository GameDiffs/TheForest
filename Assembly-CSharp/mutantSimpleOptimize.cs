using System;
using UnityEngine;

public class mutantSimpleOptimize : MonoBehaviour
{
	public GameObject[] disableList;

	private mutantScriptSetup setup;

	private bool onBool;

	private void Start()
	{
		this.setup = base.GetComponent<mutantScriptSetup>();
	}

	private void doOptimize()
	{
		if (!this.setup.ai.awayFromPlayer && !this.onBool)
		{
			this.onBool = true;
			GameObject[] array = this.disableList;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i];
				gameObject.SetActive(true);
			}
		}
		else if (this.setup.ai.awayFromPlayer && this.onBool)
		{
			this.onBool = false;
			GameObject[] array2 = this.disableList;
			for (int j = 0; j < array2.Length; j++)
			{
				GameObject gameObject2 = array2[j];
				gameObject2.SetActive(false);
			}
		}
	}
}
