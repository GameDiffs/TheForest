using System;
using UnityEngine;

public class CoopWaterColliderRoot : MonoBehaviour
{
	[SerializeField]
	private CoopWaterCollider[] _all;

	public static CoopWaterCollider[] All;

	private void Awake()
	{
		CoopWaterColliderRoot.All = this._all;
	}
}
