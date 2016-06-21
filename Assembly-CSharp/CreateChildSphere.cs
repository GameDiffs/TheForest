using System;
using UnityEngine;

public class CreateChildSphere : MonoBehaviour
{
	public Transform prefab;

	static CreateChildSphere()
	{
		DelegateSupport.RegisterFunctionType<CreateChildSphere, string>();
		DelegateSupport.RegisterFunctionType<CreateChildSphere, bool>();
		DelegateSupport.RegisterFunctionType<CreateChildSphere, Transform>();
	}

	private void Start()
	{
		if (!LevelSerializer.IsDeserializing && (double)UnityEngine.Random.value < 0.4)
		{
			Transform transform = UnityEngine.Object.Instantiate(this.prefab, base.transform.position + UnityEngine.Random.onUnitSphere * 3f, Quaternion.identity) as Transform;
			transform.parent = base.transform;
		}
	}
}
