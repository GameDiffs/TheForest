using System;
using UnityEngine;

[Serializable]
public class object_point_class
{
	public Vector2 position;

	public GameObject object1;

	public bool inRange;

	public object_point_class(Vector3 _position, GameObject _object1)
	{
		this.inRange = true;
		this.position = _position;
		this.object1 = _object1;
	}
}
