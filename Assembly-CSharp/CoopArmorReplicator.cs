using Bolt;
using System;
using UnityEngine;

public class CoopArmorReplicator : EntityBehaviour<IPlayerState>
{
	public Material[] ArmorMaterials;

	public Material[] ArmsMaterials;

	public GameObject[] Armors;

	public GameObject[] Leaves;

	public GameObject[] Bones;

	public GameObject[] Cloth;

	public GameObject ReBreather;

	public SkinnedMeshRenderer ArmsRenderer;

	public override void Attached()
	{
		if (!this.entity.isOwner)
		{
			base.state.AddCallback("Armors[]", new PropertyCallbackSimple(this.ArmorsChanged));
			base.state.AddCallback("Leaves[]", new PropertyCallbackSimple(this.LeavesChanged));
			base.state.AddCallback("Bones[]", new PropertyCallbackSimple(this.BonesChanged));
			base.state.AddCallback("ClothWeapons[]", new PropertyCallbackSimple(this.ClothChanged));
			base.state.AddCallback("Rebreater", new PropertyCallbackSimple(this.ReBreatherChanged));
			base.state.AddCallback("ArmSkin", new PropertyCallbackSimple(this.ArmSkinChanged));
		}
	}

	private void ArmSkinChanged()
	{
		this.ArmsRenderer.sharedMaterial = this.ArmsMaterials[base.state.ArmSkin - 1];
	}

	private void ReBreatherChanged()
	{
		this.ReBreather.SetActive(base.state.Rebreater);
	}

	private void ClothChanged()
	{
		for (int i = 0; i < Mathf.Min(this.Cloth.Length, base.state.ClothWeapons.Length); i++)
		{
			if (this.Cloth[i])
			{
				this.Cloth[i].GetComponent<MeshRenderer>().enabled = (base.state.ClothWeapons[i] == 1);
				this.Cloth[i].SetActive(base.state.ClothWeapons[i] == 1);
			}
		}
	}

	private void LeavesChanged()
	{
		for (int i = 0; i < Mathf.Min(base.state.Leaves.Length, this.Leaves.Length); i++)
		{
			if (this.Leaves[i])
			{
				this.Leaves[i].SetActive(base.state.Leaves[i] == 1);
			}
		}
	}

	private void BonesChanged()
	{
		for (int i = 0; i < Mathf.Min(base.state.Bones.Length, this.Bones.Length); i++)
		{
			if (this.Bones[i])
			{
				this.Bones[i].SetActive(base.state.Bones[i] == 1);
			}
		}
	}

	private void ArmorsChanged()
	{
		for (int i = 0; i < Mathf.Min(base.state.Armors.Length, this.Armors.Length); i++)
		{
			if (this.Armors[i])
			{
				if (base.state.Armors[i] > 0)
				{
					this.Armors[i].SetActive(true);
					this.Armors[i].GetComponent<Renderer>().sharedMaterial = this.ArmorMaterials[base.state.Armors[i] - 1];
				}
				else
				{
					this.Armors[i].SetActive(false);
				}
			}
		}
	}

	private void Update()
	{
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
		{
			for (int i = 0; i < this.ArmsMaterials.Length; i++)
			{
				if (this.ArmsMaterials[i] && object.ReferenceEquals(this.ArmsMaterials[i], this.ArmsRenderer.sharedMaterial))
				{
					base.state.ArmSkin = i + 1;
					break;
				}
			}
			base.state.Rebreater = this.ReBreather.activeInHierarchy;
			for (int j = 0; j < Mathf.Min(this.Cloth.Length, base.state.ClothWeapons.Length); j++)
			{
				if (this.Cloth[j])
				{
					int num = 0;
					if (this.Cloth[j].activeInHierarchy)
					{
						num = ((!this.Cloth[j].GetComponentInChildren<Renderer>().enabled) ? 0 : 1);
					}
					if (num != base.state.ClothWeapons[j])
					{
						base.state.ClothWeapons[j] = num;
					}
				}
			}
			for (int k = 0; k < Mathf.Min(base.state.Leaves.Length, this.Leaves.Length); k++)
			{
				if (this.Leaves[k])
				{
					int num2 = (!this.Leaves[k].activeInHierarchy) ? 0 : 1;
					if (num2 != base.state.Leaves[k])
					{
						base.state.Leaves[k] = num2;
					}
				}
			}
			for (int l = 0; l < Mathf.Min(base.state.Bones.Length, this.Bones.Length); l++)
			{
				if (this.Bones[l])
				{
					int num3 = (!this.Bones[l].activeInHierarchy) ? 0 : 1;
					if (num3 != base.state.Bones[l])
					{
						base.state.Bones[l] = num3;
					}
				}
			}
			for (int m = 0; m < Mathf.Min(base.state.Armors.Length, this.Armors.Length); m++)
			{
				if (this.Armors[m])
				{
					int num4 = 0;
					if (this.Armors[m].activeInHierarchy)
					{
						for (int n = 0; n < this.ArmorMaterials.Length; n++)
						{
							if (object.ReferenceEquals(this.ArmorMaterials[n], this.Armors[m].GetComponent<Renderer>().sharedMaterial))
							{
								num4 = n + 1;
								break;
							}
						}
					}
					if (num4 != base.state.Armors[m])
					{
						base.state.Armors[m] = num4;
					}
				}
			}
		}
	}
}
