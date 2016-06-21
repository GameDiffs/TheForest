using Bolt;
using PathologicalGames;
using System;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

[DoNotSerializePublic]
public class Garden : MonoBehaviour
{
	[Serializable]
	public class SeedTypes
	{
		[ItemIdPicker]
		public int _itemId;

		public GameObject _plantIcon;

		public GardenDirtPile _dirtPilePrefab;
	}

	public enum DirtPileTypes
	{
		All,
		SmallOnly
	}

	public Transform[] GrowSpots;

	public GameObject RotateIcon;

	public Garden.SeedTypes[] _seeds;

	public Garden.DirtPileTypes _dirtPileType;

	private int _currentSeedType;

	public bool Test
	{
		get;
		set;
	}

	public int BusySlots
	{
		get
		{
			return this.GrowSpots.Sum((Transform gs) => gs.childCount);
		}
	}

	private int CurrentSeedItemId
	{
		get
		{
			return this._seeds[this._currentSeedType]._itemId;
		}
	}

	private GameObject CurrentSeedPlantIcon
	{
		get
		{
			return this._seeds[this._currentSeedType]._plantIcon;
		}
	}

	private void Awake()
	{
		base.enabled = false;
		this._currentSeedType = this.NextSeedType();
	}

	private void Update()
	{
		bool flag = LocalPlayer.Inventory.Owns(this.CurrentSeedItemId);
		bool flag2 = this.BusySlots < this.GrowSpots.Length && flag;
		if (flag2)
		{
			if (TheForest.Utils.Input.GetButtonDown("Take") && LocalPlayer.Inventory.RemoveItem(this.CurrentSeedItemId, 1, true))
			{
				this.PlantSeed();
				flag2 = false;
			}
			int num = this.NextSeedType();
			if (num != this._currentSeedType)
			{
				if (!this.RotateIcon.activeSelf)
				{
					this.RotateIcon.SetActive(true);
				}
				if (TheForest.Utils.Input.GetButtonDown("Rotate"))
				{
					this.CurrentSeedPlantIcon.SetActive(false);
					this._currentSeedType = num;
				}
			}
			else if (this.RotateIcon.activeSelf)
			{
				this.RotateIcon.SetActive(false);
			}
		}
		else if (!flag)
		{
			this.CurrentSeedPlantIcon.SetActive(false);
			this._currentSeedType = this.NextSeedType();
			if (this.RotateIcon.activeSelf)
			{
				this.RotateIcon.SetActive(false);
			}
		}
		if (this.CurrentSeedPlantIcon.activeSelf != flag2)
		{
			this.CurrentSeedPlantIcon.SetActive(flag2);
		}
	}

	private void GrabEnter(GameObject grabber)
	{
		base.enabled = true;
	}

	private void GrabExit()
	{
		base.enabled = false;
		this.CurrentSeedPlantIcon.SetActive(false);
		this.RotateIcon.SetActive(false);
	}

	private int NextSeedType()
	{
		int num = this._seeds.Length;
		int num2 = this._currentSeedType;
		while (num-- > 0)
		{
			num2 = (int)Mathf.Repeat((float)(num2 + 1), (float)this._seeds.Length);
			if (LocalPlayer.Inventory.Owns(this._seeds[num2]._itemId))
			{
				break;
			}
		}
		return num2;
	}

	public void PlantSeed()
	{
		if (BoltNetwork.isRunning)
		{
			if (BoltNetwork.isClient)
			{
				LocalPlayer.Sfx.PlayDigDirtPile(base.gameObject);
			}
			GrowGarden growGarden = GrowGarden.Create(GlobalTargets.OnlyServer);
			growGarden.Garden = base.GetComponentInParent<BoltEntity>();
			growGarden.SeedNum = this._currentSeedType;
			growGarden.Send();
		}
		else
		{
			this.PlantSeed_Real(this._currentSeedType);
		}
	}

	public void PlantSeed_Real(int seedNum)
	{
		int num = 0;
		while (num < this.GrowSpots.Length && this.GrowSpots[num].childCount > 0)
		{
			num++;
		}
		if (num < this.GrowSpots.Length)
		{
			this.SpawnDirtPile(seedNum, num);
		}
	}

	private void SpawnDirtPile(int seedNum, int growSpotNum)
	{
		Transform transform = this.GrowSpots[growSpotNum].transform;
		Transform transform2 = PoolManager.Pools["misc"].Spawn(this._seeds[seedNum]._dirtPilePrefab.transform, transform.position, transform.rotation);
		GardenDirtPile component = transform2.GetComponent<GardenDirtPile>();
		component.SlotNum = growSpotNum;
		if (!BoltNetwork.isRunning)
		{
			float num = UnityEngine.Random.Range(0.7f, 1f);
			transform2.localScale = new Vector3(num, num, num);
		}
		transform2.parent = transform;
		LocalPlayer.Sfx.PlayDigDirtPile(component.gameObject);
		if (BoltNetwork.isRunning)
		{
			BoltEntity component2 = component.GetComponent<BoltEntity>();
			if (component2 && !component2.isAttached)
			{
				BoltNetwork.Attach(component2);
			}
		}
	}
}
