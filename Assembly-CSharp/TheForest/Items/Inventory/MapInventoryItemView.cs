using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/Map Item Inventory View")]
	public class MapInventoryItemView : InventoryItemView, IItemPartInventoryView
	{
		[SerializeThis]
		public List<bool> _revealedPieces = new List<bool>(4);

		public GameObject[] _piecesIIV = new GameObject[4];

		public GameObject[] _piecesRenders = new GameObject[4];

		public GameObject[] _piecesRendersHeld = new GameObject[4];

		public GameObject[] _piecesItemsHeld = new GameObject[4];

		private bool _initDone;

		private Vector3 _startHeldPosition;

		private void Awake()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				this.Init();
			}
		}

		public override void OnDeserialized()
		{
			this.Init();
			base.OnDeserialized();
		}

		public override void Init()
		{
			if (!this._initDone)
			{
				base.Init();
				this._initDone = true;
				if (!this._isCraft)
				{
					this._startHeldPosition = this._held.transform.localPosition;
				}
				this._revealedPieces.RemoveRange(4, this._revealedPieces.Count - 4);
				for (int i = 0; i < 4; i++)
				{
					if (this._revealedPieces[i])
					{
						if (!this._isCraft)
						{
							((IItemPartInventoryView)this._inventory._craftingCog.ItemViewsCache[this._itemId]).AddPiece(i);
						}
						else
						{
							this._revealedPieces[i] = true;
							this._piecesRenders[i].SetActive(true);
						}
					}
				}
			}
		}

		public void AddPiece(int pieceNum)
		{
			this._revealedPieces[pieceNum] = true;
			this._piecesRenders[pieceNum].SetActive(true);
			if (!this._isCraft)
			{
				this.Init();
				this._piecesRendersHeld[pieceNum].SetActive(true);
				this._piecesIIV[pieceNum].SetActive(false);
				if (this._revealedPieces[1] && this._revealedPieces[3])
				{
					this._held.transform.localPosition = this._startHeldPosition;
				}
				else if (this._revealedPieces[1])
				{
					this._held.transform.localPosition = this._piecesItemsHeld[1].transform.localPosition;
				}
				else if (this._revealedPieces[3])
				{
					this._held.transform.localPosition = this._piecesItemsHeld[3].transform.localPosition;
				}
				else
				{
					this._held.transform.localPosition = Vector3.Lerp(this._piecesItemsHeld[0].transform.localPosition, this._piecesItemsHeld[2].transform.localPosition, 0.5f);
				}
			}
			else
			{
				((IItemPartInventoryView)this._inventory.InventoryItemViewsCache[this._itemId][0]).AddPiece(pieceNum);
			}
		}
	}
}
