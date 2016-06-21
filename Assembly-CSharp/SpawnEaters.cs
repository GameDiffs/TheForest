using System;
using TheForest.Utils;
using UnityEngine;

public class SpawnEaters : MonoBehaviour
{
	public Transform[] spots;

	public GameObject enemyPrefab;

	private void Start()
	{
		int num = UnityEngine.Random.Range(1, this.spots.Length);
		if (LocalPlayer.AnimControl.swimming)
		{
			return;
		}
		while (num-- > 0)
		{
			int num2 = UnityEngine.Random.Range(0, this.spots.Length);
			if (this.spots[num2].childCount == 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.enemyPrefab);
				gameObject.transform.parent = this.spots[num2];
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				Animator component = gameObject.GetComponent<Animator>();
				component.CrossFade("Base Layer.feeding1", 0f, 0, UnityEngine.Random.Range(0f, 1f));
			}
		}
	}
}
