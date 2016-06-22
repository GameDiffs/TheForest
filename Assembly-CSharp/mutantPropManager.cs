using Bolt;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class mutantPropManager : EntityBehaviour
{
	public GameObject[] art;

	public GameObject[] hats;

	public GameObject[] necklaces;

	public GameObject[] loinCloth;

	public GameObject[] bracelets;

	public GameObject[] anklets;

	public Material[] material;

	public Material FireManMat;

	public Material DynamiteMat;

	public Material paleMat;

	public Material cannibalMat;

	public Material skinnyMat;

	public Material burntMat;

	public GameObject MyBody;

	public GameObject lowBody;

	public GameObject lowSkinnyBody;

	public GameObject[] MyBodyParts;

	public GameObject[] MySkinnyParts;

	private GameObject bodyParentGo;

	private GameObject skinnyParentGo;

	public GameObject props1Go;

	public GameObject props2Go;

	public GameObject props3Go;

	public GameObject leaderCape;

	public GameObject leaderCape1;

	public GameObject leaderCape2;

	public GameObject[] lights;

	[HideInInspector]
	public string SelectedMaterial = string.Empty;

	[HideInInspector]
	public Dictionary<string, Material> MaterialLookup = new Dictionary<string, Material>();

	[HideInInspector]
	public SkinnedMeshRenderer skinRenderer;

	private mutantAI ai;

	private mutantScriptSetup setup;

	private Material[] mats;

	public GameObject FireStick;

	public GameObject Club;

	public GameObject tennisBallBelt;

	public GameObject dynamiteBelt;

	public int MyRandom;

	public bool FireMan;

	public bool dynamiteMan;

	public bool pale;

	public bool female;

	public int LeaderDice;

	public int regularMaleDice;

	public int regularFemaleDice;

	public bool skipSetup;

	public bool dummySetup;

	private MaterialPropertyBlock bloodPropertyBlock;

	private void Awake()
	{
		this.bloodPropertyBlock = new MaterialPropertyBlock();
		this.skinRenderer = this.MyBody.GetComponent<SkinnedMeshRenderer>();
		this.ai = base.GetComponent<mutantAI>();
		this.setup = base.GetComponent<mutantScriptSetup>();
		if (this.MyBodyParts.Length > 0)
		{
			this.bodyParentGo = this.MyBodyParts[0].transform.parent.gameObject;
		}
		if (this.MySkinnyParts.Length > 0)
		{
			this.skinnyParentGo = this.MySkinnyParts[0].transform.parent.gameObject;
		}
		if (!this.female)
		{
			this.MaterialLookup.Add(this.FireManMat.name, this.FireManMat);
			this.MaterialLookup.Add(this.DynamiteMat.name, this.DynamiteMat);
			this.MaterialLookup.Add(this.paleMat.name, this.paleMat);
			this.MaterialLookup.Add(this.cannibalMat.name, this.cannibalMat);
			this.MaterialLookup.Add(this.skinnyMat.name, this.skinnyMat);
			Material[] array = this.material;
			for (int i = 0; i < array.Length; i++)
			{
				Material material = array[i];
				if (!this.MaterialLookup.ContainsKey(material.name))
				{
					this.MaterialLookup.Add(material.name, material);
				}
			}
		}
	}

	public void setRegularMale(int dice)
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.LeaderDice = UnityEngine.Random.Range(0, 3);
		this.regularMaleDice = UnityEngine.Random.Range(0, 5);
		if (this.dummySetup)
		{
			this.regularMaleDice = dice;
		}
		if (this.leaderCape)
		{
			this.leaderCape.SetActive(false);
		}
		if (this.leaderCape1)
		{
			this.leaderCape1.SetActive(false);
		}
		if (this.leaderCape2)
		{
			this.leaderCape2.SetActive(false);
		}
		if (this.dummySetup)
		{
			this.Club.SetActive(false);
		}
		switch (this.regularMaleDice)
		{
		case 0:
			this.hats[0].SetActive(false);
			this.props1Go.SetActive(true);
			this.props2Go.SetActive(false);
			this.props3Go.SetActive(false);
			this.SetSkin(this.material[0]);
			break;
		case 1:
			this.hats[0].SetActive(true);
			this.props1Go.SetActive(false);
			this.props2Go.SetActive(true);
			this.props3Go.SetActive(false);
			this.SetSkin(this.material[1]);
			break;
		case 2:
			this.hats[0].SetActive(false);
			this.props1Go.SetActive(false);
			this.props2Go.SetActive(false);
			this.props3Go.SetActive(true);
			this.SetSkin(this.material[2]);
			break;
		case 3:
			this.hats[0].SetActive(false);
			this.props1Go.SetActive(false);
			this.props2Go.SetActive(false);
			this.props3Go.SetActive(true);
			this.SetSkin(this.material[3]);
			break;
		case 4:
			this.hats[0].SetActive(false);
			this.props1Go.SetActive(false);
			this.props2Go.SetActive(true);
			this.props3Go.SetActive(false);
			this.SetSkin(this.material[0]);
			break;
		}
	}

	private void OnDespawned()
	{
		if (this.ai.male && this.skinRenderer && !this.dummySetup)
		{
			this.skinRenderer.SetBlendShapeWeight(28, 0f);
		}
		if (this.leaderCape)
		{
			this.leaderCape.SetActive(false);
		}
		if (this.leaderCape1)
		{
			this.leaderCape1.SetActive(false);
		}
		if (this.leaderCape2)
		{
			this.leaderCape2.SetActive(false);
		}
		if (this.tennisBallBelt)
		{
			this.tennisBallBelt.SetActive(false);
		}
		if (this.dynamiteBelt)
		{
			this.dynamiteBelt.SetActive(false);
		}
		this.FireMan = false;
		this.dynamiteMan = false;
		this.pale = false;
		this.female = false;
	}

	public void enableLeaderProps(int dice)
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		if (this.dummySetup)
		{
			this.LeaderDice = dice;
		}
		if (this.LeaderDice == 0)
		{
			this.leaderCape.SetActive(true);
		}
		if (this.LeaderDice == 1)
		{
			this.leaderCape1.SetActive(true);
		}
		if (this.LeaderDice == 2)
		{
			this.leaderCape2.SetActive(true);
		}
		this.FireStick.SetActive(false);
		this.Club.SetActive(true);
		if (this.dummySetup)
		{
			this.Club.SetActive(false);
			this.skinnyParentGo.SetActive(false);
			this.bodyParentGo.SetActive(true);
		}
		else
		{
			this.SetBlendShapeWeight(28, 0);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void enableFiremanProps()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.FireMan = true;
		this.Club.SetActive(false);
		if (this.dynamiteMan)
		{
			if (this.dynamiteBelt)
			{
				this.dynamiteBelt.SetActive(true);
			}
		}
		else if (this.tennisBallBelt)
		{
			this.tennisBallBelt.SetActive(true);
		}
		if (this.dummySetup)
		{
			this.FireStick.SetActive(false);
		}
		else
		{
			this.FireStick.SetActive(true);
		}
		if (this.props1Go)
		{
			this.props1Go.SetActive(false);
		}
		if (this.props2Go)
		{
			this.props2Go.SetActive(true);
		}
		if (this.props3Go)
		{
			this.props3Go.SetActive(false);
		}
		if (this.dummySetup)
		{
			this.skinnyParentGo.SetActive(false);
			this.bodyParentGo.SetActive(true);
		}
		else
		{
			this.SetBlendShapeWeight(28, 0);
		}
		if (this.dynamiteMan)
		{
			this.SetSkin(this.DynamiteMat);
		}
		else
		{
			this.SetSkin(this.FireManMat);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void enableDefaultProps()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.Club.SetActive(true);
		this.FireStick.SetActive(false);
		if (this.dummySetup)
		{
			this.skinnyParentGo.SetActive(false);
			this.bodyParentGo.SetActive(true);
		}
		else
		{
			this.SetBlendShapeWeight(28, 0);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void enableMaleSkinnyProps()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.resetProps();
		this.Club.SetActive(false);
		this.FireStick.SetActive(false);
		if (this.dummySetup)
		{
			this.skinnyParentGo.SetActive(true);
			this.bodyParentGo.SetActive(false);
		}
		else
		{
			this.SetBlendShapeWeight(28, 100);
		}
		this.SetSkin(this.skinnyMat);
		GameObject[] array = this.hats;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
		if (this.props1Go)
		{
			this.props1Go.SetActive(false);
		}
		if (this.props2Go)
		{
			this.props2Go.SetActive(false);
		}
		if (this.props3Go)
		{
			this.props3Go.SetActive(false);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void enableMaleSkinnyPaleProps()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.resetProps();
		this.Club.SetActive(false);
		this.FireStick.SetActive(false);
		if (this.dummySetup)
		{
			this.skinnyParentGo.SetActive(true);
			this.bodyParentGo.SetActive(false);
		}
		else
		{
			this.SetBlendShapeWeight(28, 100);
		}
		this.SetSkin(this.paleMat);
		GameObject[] array = this.hats;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
		if (this.props1Go)
		{
			this.props1Go.SetActive(false);
		}
		if (this.props2Go)
		{
			this.props2Go.SetActive(false);
		}
		if (this.props3Go)
		{
			this.props3Go.SetActive(false);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void enablePaleProps()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.resetProps();
		this.pale = true;
		this.Club.SetActive(false);
		this.FireStick.SetActive(false);
		if (this.dummySetup)
		{
			this.skinnyParentGo.SetActive(false);
			this.bodyParentGo.SetActive(true);
		}
		else
		{
			this.SetBlendShapeWeight(28, 0);
		}
		this.SetSkin(this.paleMat);
		GameObject[] array = this.hats;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			gameObject.SetActive(false);
		}
		if (this.props1Go)
		{
			this.props1Go.SetActive(false);
		}
		if (this.props2Go)
		{
			this.props2Go.SetActive(false);
		}
		if (this.props3Go)
		{
			this.props3Go.SetActive(false);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetSkin(Material mat)
	{
		this.skinRenderer.material = mat;
		if (this.lowBody)
		{
			this.lowBody.GetComponent<SkinnedMeshRenderer>().material = mat;
		}
		if (this.lowSkinnyBody)
		{
			this.lowSkinnyBody.GetComponent<SkinnedMeshRenderer>().material = mat;
		}
		if (this.MyBodyParts.Length > 0)
		{
			GameObject[] myBodyParts = this.MyBodyParts;
			for (int i = 0; i < myBodyParts.Length; i++)
			{
				GameObject gameObject = myBodyParts[i];
				gameObject.GetComponent<Renderer>().material = mat;
			}
		}
		if (this.MySkinnyParts.Length > 0)
		{
			GameObject[] mySkinnyParts = this.MySkinnyParts;
			for (int j = 0; j < mySkinnyParts.Length; j++)
			{
				GameObject gameObject2 = mySkinnyParts[j];
				gameObject2.GetComponent<Renderer>().material = mat;
			}
		}
		if (!this.female)
		{
			this.SelectedMaterial = mat.name;
		}
	}

	private void spawnRandomArt()
	{
		Vector3 pos = base.transform.position + base.transform.forward * 2f;
		Transform transform = PoolManager.Pools["misc"].Spawn(this.art[UnityEngine.Random.Range(0, this.art.Length)].transform, pos, base.transform.rotation);
		transform.eulerAngles = new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
		spawnTimeout component = transform.GetComponent<spawnTimeout>();
		component.despawnTime = 1000f;
		component.invokeDespawn();
	}

	public void SetBlendShapeWeight(int index, int value)
	{
		if (!this.skinRenderer)
		{
			this.skinRenderer = this.MyBody.GetComponent<SkinnedMeshRenderer>();
		}
		this.skinRenderer.SetBlendShapeWeight(index, (float)value);
		if (BoltNetwork.isServer && !this.female && index == 28)
		{
			this.entity.GetState<IMutantMaleState>().BlendShapeWeight0 = (float)value;
		}
	}

	public void resetProps()
	{
		if (this.leaderCape)
		{
			this.leaderCape.SetActive(false);
		}
		if (this.leaderCape1)
		{
			this.leaderCape1.SetActive(false);
		}
		if (this.leaderCape2)
		{
			this.leaderCape2.SetActive(false);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void enableLights()
	{
		GameObject[] array = this.lights;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			gameObject.SetActive(true);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void disableLights()
	{
		GameObject[] array = this.lights;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			gameObject.SetActive(false);
		}
		if (!this.female)
		{
			base.SendMessageUpwards("UpdateProps", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void setBurntSkin()
	{
		if (this.burntMat)
		{
			this.SetSkin(this.burntMat);
		}
	}

	[DebuggerHidden]
	public IEnumerator setupFeedingProps(int n)
	{
		mutantPropManager.<setupFeedingProps>c__Iterator80 <setupFeedingProps>c__Iterator = new mutantPropManager.<setupFeedingProps>c__Iterator80();
		<setupFeedingProps>c__Iterator.n = n;
		<setupFeedingProps>c__Iterator.<$>n = n;
		<setupFeedingProps>c__Iterator.<>f__this = this;
		return <setupFeedingProps>c__Iterator;
	}

	private void dropHeldMeat()
	{
		if (this.setup.heldMeat.activeSelf)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.setup.spawnedMeat, this.setup.heldMeat.transform.position, this.setup.heldMeat.transform.rotation) as GameObject;
			this.setup.heldMeat.SetActive(false);
		}
		if (this.setup.ai.fireman)
		{
			this.setup.fireWeapon.SetActive(true);
		}
		if (this.setup.ai.male && !this.setup.ai.pale && !this.setup.ai.maleSkinny && !this.setup.ai.fireman)
		{
			this.setup.charLeftWeaponGo.SetActive(true);
		}
	}

	public void setDynamiteMan()
	{
		this.dynamiteMan = true;
	}

	private void setSkinDamage1(float val)
	{
		this.applySkinDamage("_Damage1", val);
	}

	private void setSkinDamage2(float val)
	{
		this.applySkinDamage("_Damage2", val);
	}

	private void setSkinDamage3(float val)
	{
		this.applySkinDamage("_Damage3", val);
	}

	private void setSkinDamage4(float val)
	{
		this.applySkinDamage("_Damage4", val);
	}

	private void setSkinDamageProperty(MaterialPropertyBlock block)
	{
		if (this.MyBodyParts.Length > 0)
		{
			GameObject[] myBodyParts = this.MyBodyParts;
			for (int i = 0; i < myBodyParts.Length; i++)
			{
				GameObject gameObject = myBodyParts[i];
				Renderer component = gameObject.GetComponent<Renderer>();
				component.SetPropertyBlock(block);
			}
		}
		if (this.MySkinnyParts.Length > 0)
		{
			GameObject[] mySkinnyParts = this.MySkinnyParts;
			for (int j = 0; j < mySkinnyParts.Length; j++)
			{
				GameObject gameObject2 = mySkinnyParts[j];
				Renderer component2 = gameObject2.GetComponent<Renderer>();
				component2.SetPropertyBlock(block);
			}
		}
	}

	private void applySkinDamage(string t, float s)
	{
		if (this.MyBodyParts.Length > 0)
		{
			GameObject[] myBodyParts = this.MyBodyParts;
			for (int i = 0; i < myBodyParts.Length; i++)
			{
				GameObject gameObject = myBodyParts[i];
				Renderer component = gameObject.GetComponent<Renderer>();
				component.GetPropertyBlock(this.bloodPropertyBlock);
				this.bloodPropertyBlock.SetFloat(t, s);
				component.SetPropertyBlock(this.bloodPropertyBlock);
			}
		}
		if (this.MySkinnyParts.Length > 0)
		{
			GameObject[] mySkinnyParts = this.MySkinnyParts;
			for (int j = 0; j < mySkinnyParts.Length; j++)
			{
				GameObject gameObject2 = mySkinnyParts[j];
				gameObject2.GetComponent<Renderer>().material.SetFloat(t, s);
			}
		}
	}
}
