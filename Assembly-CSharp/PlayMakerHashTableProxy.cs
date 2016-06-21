using System;
using System.Collections;

public class PlayMakerHashTableProxy : PlayMakerCollectionProxy
{
	public Hashtable _hashTable;

	public Hashtable hashTable
	{
		get
		{
			return this._hashTable;
		}
	}

	public void Awake()
	{
		this._hashTable = new Hashtable();
		this.PreFillHashTable();
	}

	public bool isCollectionDefined()
	{
		return this.hashTable != null;
	}

	public void InspectorEdit(int index)
	{
		base.dispatchEvent(this.setEvent, index, "int");
	}

	private void PreFillHashTable()
	{
		for (int i = 0; i < this.preFillKeyList.Count; i++)
		{
			switch (this.preFillType)
			{
			case PlayMakerCollectionProxy.VariableEnum.GameObject:
				this.hashTable[this.preFillKeyList[i]] = this.preFillGameObjectList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Int:
				this.hashTable[this.preFillKeyList[i]] = this.preFillIntList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Float:
				this.hashTable[this.preFillKeyList[i]] = this.preFillFloatList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.String:
				this.hashTable[this.preFillKeyList[i]] = this.preFillStringList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Bool:
				this.hashTable[this.preFillKeyList[i]] = this.preFillBoolList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Vector3:
				this.hashTable[this.preFillKeyList[i]] = this.preFillVector3List[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Rect:
				this.hashTable[this.preFillKeyList[i]] = this.preFillRectList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Quaternion:
				this.hashTable[this.preFillKeyList[i]] = this.preFillQuaternionList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Color:
				this.hashTable[this.preFillKeyList[i]] = this.preFillColorList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Material:
				this.hashTable[this.preFillKeyList[i]] = this.preFillMaterialList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Texture:
				this.hashTable[this.preFillKeyList[i]] = this.preFillTextureList[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.Vector2:
				this.hashTable[this.preFillKeyList[i]] = this.preFillVector2List[i];
				break;
			case PlayMakerCollectionProxy.VariableEnum.AudioClip:
				this.hashTable[this.preFillKeyList[i]] = this.preFillAudioClipList[i];
				break;
			}
		}
	}
}
