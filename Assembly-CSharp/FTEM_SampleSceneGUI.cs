using System;
using UnityEngine;

public class FTEM_SampleSceneGUI : MonoBehaviour
{
	public GUIText prefabName;

	public GameObject[] particlePrefab;

	public int particleNum;

	private GameObject effectPrefab;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (this.particleNum < 3)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, 1000f))
				{
					this.effectPrefab = (GameObject)UnityEngine.Object.Instantiate(this.particlePrefab[this.particleNum], new Vector3(raycastHit.point.x, raycastHit.point.y + 3f, raycastHit.point.z), Quaternion.Euler(0f, 0f, 0f));
				}
			}
			else
			{
				Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit2;
				if (Physics.Raycast(ray2, out raycastHit2, 1000f))
				{
					this.effectPrefab = (GameObject)UnityEngine.Object.Instantiate(this.particlePrefab[this.particleNum], new Vector3(raycastHit2.point.x, raycastHit2.point.y, raycastHit2.point.z), Quaternion.Euler(0f, 0f, 0f));
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			UnityEngine.Object.Destroy(this.effectPrefab);
			this.particleNum--;
			if (this.particleNum < 0)
			{
				this.particleNum = this.particlePrefab.Length - 1;
			}
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			UnityEngine.Object.Destroy(this.effectPrefab);
			this.particleNum++;
			if (this.particleNum > this.particlePrefab.Length - 1)
			{
				this.particleNum = 0;
			}
		}
		this.prefabName.text = this.particlePrefab[this.particleNum].name;
	}
}
