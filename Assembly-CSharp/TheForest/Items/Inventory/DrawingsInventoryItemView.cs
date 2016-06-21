using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/Drawings Inventory View")]
	public class DrawingsInventoryItemView : InventoryItemView
	{
		[SerializeThis]
		public List<int> _ids;

		[SerializeThis]
		public List<int> _usedIds;

		[SerializeThis]
		private int _idsCount;

		[SerializeThis]
		private int _usedIdsCount;

		private bool _initDone;

		private void Awake()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				this.Init();
			}
		}

		public override void OnSerializing()
		{
			this._idsCount = this._ids.Count;
			this._usedIdsCount = this._usedIds.Count;
			base.OnSerializing();
		}

		public override void OnDeserialized()
		{
			this._ids.RemoveRange(this._idsCount, this._ids.Count - this._idsCount);
			this._usedIds.RemoveRange(this._usedIdsCount, this._usedIds.Count - this._usedIdsCount);
			this.Init();
			base.OnDeserialized();
		}

		public void SetLast(int id)
		{
			this._ids.Add(id);
			this.RefreshMaterial();
		}

		public int PopLast()
		{
			int num;
			if (this._ids.Count > 0)
			{
				num = this._ids[this._ids.Count - 1];
				this._ids.RemoveAt(this._ids.Count - 1);
				this._usedIds.Add(num);
			}
			else
			{
				num = 0;
			}
			this.RefreshMaterial();
			return num;
		}

		public bool HasId(int id)
		{
			return this._ids.Contains(id) || this._usedIds.Contains(id);
		}

		public override void Init()
		{
			if (!this._initDone)
			{
				base.Init();
				this._initDone = true;
				this.RefreshMaterial();
			}
		}

		private void RefreshMaterial()
		{
			Material normalMaterial = base.NormalMaterial;
			base.NormalMaterial = Prefabs.Instance.TimmyDrawingsMats[(this._ids.Count <= 0) ? 0 : this._ids[this._ids.Count - 1]];
			Renderer component = base.GetComponent<Renderer>();
			if (component && component.sharedMaterial.Equals(normalMaterial))
			{
				component.sharedMaterial = base.NormalMaterial;
			}
			this._held.GetComponent<Renderer>().sharedMaterial = base.NormalMaterial;
		}
	}
}
