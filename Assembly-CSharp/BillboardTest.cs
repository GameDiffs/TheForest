using System;
using UnityEngine;

[RequireComponent(typeof(CustomBillboard))]
public class BillboardTest : MonoBehaviour
{
	public float Radius = 5f;

	public int Count = 5;

	private void Awake()
	{
		CustomBillboard component = base.GetComponent<CustomBillboard>();
		for (int i = 0; i < this.Count; i++)
		{
			Vector3 point = new Vector3(UnityEngine.Random.Range(0f, this.Radius), 0f, 0f);
			Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			component.Register(rotation * point, base.transform.rotation.y);
		}
	}
}
