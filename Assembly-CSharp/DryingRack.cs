using Bolt;
using System;
using TheForest.Buildings.Interfaces;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class DryingRack : MonoBehaviour, IWetable
{
	[Serializable]
	public class FoodItem
	{
		[ItemIdPicker]
		public int _itemId;

		public Cook _cookPrefab;
	}

	public DryingRack.FoodItem[] foodItems;

	public GameObject CookHeldIcon;

	public BoxCollider dryingGrid;

	public Vector2 gridChunkSize;

	private PlayerInventory Player;

	private void Start()
	{
		base.enabled = false;
	}

	private void GrabEnter(GameObject grabber)
	{
		if (this.Player == null)
		{
			this.Player = grabber.transform.root.gameObject.GetComponent<PlayerInventory>();
			base.enabled = true;
		}
	}

	private void GrabExit()
	{
		this.Player = null;
		this.CookHeldIcon.SetActive(false);
	}

	private void Update()
	{
		if (this.Player != null)
		{
			if (this.Player.EquipmentSlots[0] != null)
			{
				int num = this.foodItems.FindIndex((DryingRack.FoodItem fi) => fi._itemId == this.Player.EquipmentSlots[0]._itemId);
				if (this.CookHeldIcon.activeSelf != num >= 0)
				{
					this.CookHeldIcon.SetActive(num >= 0);
				}
				if (num >= 0 && TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					this.Player.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
					this.SpawnFoodPrefab(this.foodItems[num]._cookPrefab);
					this.CookHeldIcon.SetActive(false);
				}
			}
		}
		else if (this.Player == null)
		{
			base.enabled = false;
		}
	}

	private void SpawnFoodPrefab(Cook foodPrefab)
	{
		Vector3 center = this.dryingGrid.center;
		Vector3 vector = this.dryingGrid.size * 0.5f;
		Vector3 vector2 = center;
		Vector3 vector3 = center;
		vector2.x += vector.x;
		vector2.z += vector.z;
		vector3.x -= vector.x;
		vector3.z -= vector.z;
		Vector3 position = new Vector3(UnityEngine.Random.Range(vector3.x, vector2.x), center.y, UnityEngine.Random.Range(vector3.z, vector2.z));
		position.x -= position.x % this.gridChunkSize.x;
		position.z -= position.z % this.gridChunkSize.y;
		if (BoltNetwork.isRunning)
		{
			PlaceConstruction placeConstruction = PlaceConstruction.Create(GlobalTargets.OnlyServer);
			placeConstruction.PrefabId = foodPrefab.GetComponent<BoltEntity>().prefabId;
			placeConstruction.Position = this.dryingGrid.transform.TransformPoint(position);
			placeConstruction.Rotation = Quaternion.identity;
			placeConstruction.Parent = base.transform.parent.GetComponent<BoltEntity>();
			placeConstruction.Send();
		}
		else
		{
			Cook cook = (Cook)UnityEngine.Object.Instantiate(foodPrefab, this.dryingGrid.transform.TransformPoint(position), Quaternion.identity);
			cook.transform.parent = base.transform.parent;
		}
	}

	public void GotClean()
	{
		UnityEngine.Object.Destroy(this.CookHeldIcon);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
