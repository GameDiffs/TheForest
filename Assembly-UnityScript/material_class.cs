using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class material_class
{
	public bool active;

	public bool foldout;

	public List<Material> material;

	public List<int> combine_count;

	public List<GameObject> combine_parent;

	public value_class material_value;

	public material_class()
	{
		this.material = new List<Material>();
		this.combine_count = new List<int>();
		this.combine_parent = new List<GameObject>();
		this.material_value = new value_class();
		this.add_material(0);
	}

	public override int set_material(GameObject @object, int material_number)
	{
		MeshRenderer meshRenderer = (MeshRenderer)@object.GetComponent(typeof(MeshRenderer));
		float time = UnityEngine.Random.Range((float)0, 1f);
		material_number = Mathf.FloorToInt(this.material_value.curve.Evaluate(time) * (float)this.material.Count);
		if (material_number > this.material.Count - 1)
		{
			material_number = this.material.Count - 1;
		}
		int arg_CD_0;
		if (!this.material[material_number])
		{
			arg_CD_0 = 0;
		}
		else if (meshRenderer)
		{
			if (meshRenderer.sharedMaterials.Length == 0)
			{
				arg_CD_0 = 0;
			}
			else
			{
				meshRenderer.sharedMaterial = this.material[material_number];
				arg_CD_0 = material_number;
			}
		}
		else
		{
			arg_CD_0 = 0;
		}
		return arg_CD_0;
	}

	public override void add_material(int index)
	{
		Material item = null;
		GameObject item2 = null;
		this.material.Insert(index, item);
		this.material_value.add_value(index, (float)50);
		this.combine_count.Insert(index, 0);
		this.combine_parent.Insert(index, item2);
	}

	public override void erase_material(int index)
	{
		if (this.material.Count > 0)
		{
			this.material.RemoveAt(index);
			this.material_value.erase_value(index);
			this.combine_count.RemoveAt(index);
			this.combine_parent.RemoveAt(index);
		}
	}

	public override void clear_material()
	{
		this.material.Clear();
		this.material_value.clear_value();
		this.combine_count.Clear();
		this.combine_parent.Clear();
	}
}
