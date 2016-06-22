using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class tree_class
{
	public bool foldout;

	public bool interface_display;

	public int prototypeindex;

	public int placed;

	[NonSerialized]
	public tree_class placed_reference;

	public string swap_text;

	public bool swap_select;

	public bool copy_select;

	public Color color_tree;

	public precolor_range_class precolor_range;

	public bool scale_foldout;

	public bool link_start;

	public bool link_end;

	public float width_start;

	public float width_end;

	public float height_start;

	public float height_end;

	public float height;

	public float unlink;

	public bool random_position;

	public bool distance_foldout;

	public Vector3 min_distance;

	public distance_level_enum distance_level;

	public distance_mode_enum distance_mode;

	public rotation_mode_enum distance_rotation_mode;

	public bool distance_include_scale;

	public bool distance_include_scale_group;

	public bool data_foldout;

	public int mesh_length;

	public int mesh_triangles;

	public int mesh_combine;

	public Vector3 mesh_size;

	public List<distance_class> objects_placed;

	public prefilter_class prefilter;

	public bool raycast;

	public int layerMask;

	public float ray_length;

	public float cast_height;

	public float ray_radius;

	public Vector3 ray_direction;

	public raycast_mode_enum raycast_mode;

	public tree_class(terraincomposer_save script, bool new_filter)
	{
		this.interface_display = true;
		this.swap_text = "S";
		this.color_tree = new Color((float)1, (float)1, (float)1, (float)1);
		this.precolor_range = new precolor_range_class(1, false);
		this.link_start = true;
		this.link_end = true;
		this.width_start = (float)1;
		this.width_end = 2.5f;
		this.height_start = (float)1;
		this.height_end = 2.5f;
		this.unlink = 0.25f;
		this.random_position = true;
		this.distance_include_scale = true;
		this.distance_include_scale_group = true;
		this.objects_placed = new List<distance_class>();
		this.prefilter = new prefilter_class();
		this.ray_length = (float)20;
		this.cast_height = (float)20;
		this.ray_radius = (float)1;
		this.ray_direction = new Vector3((float)0, (float)-1, (float)0);
		if (new_filter)
		{
			script.add_filter(0, this.prefilter);
			script.filter[script.filter.Count - 1].type = condition_type_enum.Random;
			script.add_subfilter(0, script.filter[script.filter.Count - 1].presubfilter);
			script.subfilter[script.subfilter.Count - 1].type = condition_type_enum.Random;
			script.subfilter[script.subfilter.Count - 1].from_tree = true;
		}
		this.precolor_range.color_range[0].color_start = new Color(0.75f, 0.75f, 0.75f);
		this.precolor_range.color_range[0].color_end = new Color((float)1, (float)1, (float)1);
	}

	public override void count_mesh(GameObject object1)
	{
		if (object1)
		{
			MeshFilter meshFilter = (MeshFilter)object1.GetComponent(typeof(MeshFilter));
			if (meshFilter)
			{
				Mesh sharedMesh = meshFilter.sharedMesh;
				if (sharedMesh)
				{
					Vector3[] vertices = sharedMesh.vertices;
					this.mesh_size = sharedMesh.bounds.size;
					int[] triangles = sharedMesh.triangles;
					if (vertices != null)
					{
						this.mesh_length = vertices.Length;
						this.mesh_combine = (int)Mathf.Round((float)(64000 / this.mesh_length));
						this.mesh_triangles = triangles.Length;
					}
					else
					{
						Debug.Log("vertices not found, cannot display data");
					}
				}
				else
				{
					Debug.Log("mesh not found, cannot display data");
				}
			}
			else
			{
				Debug.Log("meshfilter not found, cannot display data");
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
