using System;
using TheForest.Items.Core;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	public class ItemStorageProxy : MonoBehaviour
	{
		public bool _interactiveViews;

		public ItemStorage _storage;

		public Vector2 _xRange = new Vector2(-0.1f, 0.1f);

		public Vector2 _yRange = new Vector2(0f, 0.02f);

		public Vector2 _zRange = new Vector2(-0.15f, 0.15f);

		public Vector3 _contentRotationAngle = new Vector3(90f, -180f, -180f);

		public LayerMask _spawnedLayer = 0;

		private int _contentVersion = -1;

		private GameObject _contentRoot;

		private Quaternion _contentRotation;

		private void Awake()
		{
			this._contentRotation = Quaternion.Euler(this._contentRotationAngle);
		}

		private void Update()
		{
			this.CheckContentVersion();
		}

		public void CheckContentVersion()
		{
			if (this._contentVersion != this._storage.ContentVersion)
			{
				this.RefreshItemViews();
				this._contentVersion = this._storage.ContentVersion;
			}
		}

		private void RefreshItemViews()
		{
			if (this._contentRoot)
			{
				UnityEngine.Object.Destroy(this._contentRoot);
			}
			this._contentRoot = new GameObject("content");
			this._contentRoot.transform.parent = base.transform;
			this._contentRoot.transform.localPosition = Vector3.zero;
			this._contentRoot.transform.localRotation = this._contentRotation;
			this._contentRoot.transform.localScale = new Vector3(1f / base.transform.lossyScale.x, 1f / base.transform.lossyScale.y, 1f / base.transform.lossyScale.z);
			for (int i = 0; i < this._storage.UsedSlots.Count; i++)
			{
				this.SpawnView(this._storage.UsedSlots[i]._itemId);
			}
		}

		private void SpawnView(int itemId)
		{
			InventoryItemView inventoryItemView = LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0];
			Renderer component = inventoryItemView.GetComponent<Renderer>();
			Vector3 vector;
			if (component)
			{
				vector = component.bounds.center;
				vector.y = component.bounds.min.y;
				vector = inventoryItemView.transform.position - vector;
			}
			else
			{
				vector = Vector3.zero;
				vector.y = 0.02f;
			}
			Vector3 vector2 = new Vector3(UnityEngine.Random.Range(this._xRange.x, this._xRange.y), UnityEngine.Random.Range(this._yRange.x, this._yRange.y), UnityEngine.Random.Range(this._zRange.x, this._zRange.y));
			vector2 = Vector3.zero;
			vector2 += vector;
			InventoryItemView inventoryItemView2 = UnityEngine.Object.Instantiate<InventoryItemView>(inventoryItemView);
			GameObject gameObject = inventoryItemView2.gameObject;
			gameObject.SetActive(true);
			gameObject.transform.localScale = inventoryItemView.transform.lossyScale;
			gameObject.transform.parent = this._contentRoot.transform;
			gameObject.transform.localRotation = inventoryItemView.transform.localRotation;
			gameObject.transform.localPosition = vector2;
			gameObject.layer = this.GetLayerNum();
			gameObject.AddComponent<AmplifyMotionObject>();
			Debug.DrawLine(gameObject.transform.position, gameObject.transform.position - gameObject.transform.TransformDirection(vector), Color.red, 20f);
			if (this._interactiveViews)
			{
				inventoryItemView2._isCraft = true;
				inventoryItemView2.Init();
			}
			else
			{
				UnityEngine.Object.Destroy(inventoryItemView2);
				UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
			}
		}

		private int GetLayerNum()
		{
			int num = 0;
			int i = this._spawnedLayer.value;
			while (i > 1)
			{
				i >>= 1;
				num++;
			}
			return num;
		}
	}
}
