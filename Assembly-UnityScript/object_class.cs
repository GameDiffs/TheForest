using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class object_class
{
	public bool foldout;

	public bool prefab;

	public bool data_foldout;

	public bool transform_foldout;

	public bool settings_foldout;

	public bool distance_foldout;

	public bool rotation_foldout;

	public bool rotation_map_foldout;

	public bool random_position;

	public GameObject object1;

	public GameObject object2;

	public string name;

	public bool equal_density;

	public Color color_object;

	public Transform parent;

	public bool parent_clear;

	public string parent_name;

	public bool combine;

	public bool combine_total;

	public int place_max;

	public bool place_maximum;

	public bool place_maximum_total;

	public bool parent_set;

	public Vector3 scale_start;

	public Vector3 scale_end;

	public float unlink_y;

	public float unlink_z;

	public bool unlink_foldout;

	public bool scale_link;

	public bool scale_link_start_y;

	public bool scale_link_end_y;

	public bool scale_link_start_z;

	public bool scale_link_end_z;

	public AnimationCurve scaleCurve;

	public Vector3 rotation_start;

	public Vector3 rotation_end;

	public bool rotation_link;

	public bool rotation_link_start_y;

	public bool rotation_link_end_y;

	public bool rotation_link_start_z;

	public bool rotation_link_end_z;

	public Vector3 position_start;

	public Vector3 position_end;

	public bool terrain_height;

	public bool terrain_rotate;

	public bool scale_steps;

	public Vector3 scale_step;

	public bool position_steps;

	public Vector3 position_step;

	public Vector3 parent_rotation;

	public bool look_at_parent;

	public bool rotation_steps;

	public Vector3 rotation_step;

	[NonSerialized]
	public rotation_map_class rotation_map;

	public Vector3 min_distance;

	public distance_level_enum distance_level;

	public distance_mode_enum distance_mode;

	public rotation_mode_enum distance_rotation_mode;

	public bool distance_include_scale;

	public bool distance_include_scale_group;

	public List<distance_class> objects_placed;

	public int placed;

	[NonSerialized]
	public object_class placed_reference;

	public int placed_prelayer;

	public int mesh_length;

	public int mesh_triangles;

	public int mesh_combine;

	public Vector3 mesh_size;

	public material_class object_material;

	public GameObject combine_parent;

	public string combine_parent_name;

	public bool combine_parent_name_input;

	public float value;

	public int prelayer_index;

	public bool prelayer_created;

	public string swap_text;

	public bool swap_select;

	public bool copy_select;

	[NonSerialized]
	public List<object_class> object_child;

	public Texture2D preview_texture;

	public bool pivot_center;

	public bool raycast;

	public int layerMask;

	public float ray_length;

	public float cast_height;

	public float ray_radius;

	public Vector3 ray_direction;

	public raycast_mode_enum raycast_mode;

	public bool objectStream;

	public int objectIndex;

	public float slopeY;

	public float sphereOverlapRadius;

	public float sphereOverlapHeight;

	public object_class()
	{
		this.random_position = true;
		this.equal_density = true;
		this.color_object = new Color((float)2, (float)2, (float)2, (float)1);
		this.parent_clear = true;
		this.combine = true;
		this.combine_total = true;
		this.place_max = 1;
		this.scale_start = new Vector3((float)1, (float)1, (float)1);
		this.scale_end = new Vector3((float)1, (float)1, (float)1);
		this.unlink_y = 0.25f;
		this.unlink_z = 0.25f;
		this.scale_link = true;
		this.scale_link_start_y = true;
		this.scale_link_end_y = true;
		this.scale_link_start_z = true;
		this.scale_link_end_z = true;
		this.scaleCurve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.rotation_start = new Vector3((float)0, (float)0, (float)0);
		this.rotation_end = new Vector3((float)0, (float)0, (float)0);
		this.position_start = new Vector3((float)0, (float)0, (float)0);
		this.position_end = new Vector3((float)0, (float)0, (float)0);
		this.terrain_height = true;
		this.rotation_map = new rotation_map_class();
		this.distance_include_scale = true;
		this.distance_include_scale_group = true;
		this.objects_placed = new List<distance_class>();
		this.object_material = new material_class();
		this.combine_parent_name = string.Empty;
		this.swap_text = "S";
		this.object_child = new List<object_class>();
		this.ray_length = (float)20;
		this.cast_height = (float)20;
		this.ray_radius = (float)1;
		this.ray_direction = new Vector3((float)0, (float)-1, (float)0);
	}

	public override void count_mesh()
	{
		if (this.object1)
		{
			MeshFilter meshFilter = (MeshFilter)this.object1.GetComponent(typeof(MeshFilter));
			Mesh mesh = null;
			Vector3[] array = null;
			int[] array2 = null;
			if (meshFilter)
			{
				mesh = meshFilter.sharedMesh;
			}
			if (mesh)
			{
				array = mesh.vertices;
				this.mesh_size = mesh.bounds.size;
				array2 = mesh.triangles;
			}
			if (array != null)
			{
				this.mesh_length = array.Length;
				this.mesh_combine = (int)Mathf.Round((float)(64000 / this.mesh_length));
				this.mesh_triangles = array2.Length;
			}
		}
		else
		{
			this.mesh_combine = 0;
			this.mesh_size = new Vector3((float)0, (float)0, (float)0);
			this.mesh_length = 0;
			this.mesh_triangles = 0;
		}
	}
}
