using System;
using UnityEngine;

[Serializable]
public class distance_class
{
	public Vector3 position;

	public Vector3 rotation;

	public Vector3 min_distance;

	public Vector3 min_distance_rotation_group;

	public distance_mode_enum distance_mode;

	public rotation_mode_enum distance_rotation;

	public bool rotation_group;
}
