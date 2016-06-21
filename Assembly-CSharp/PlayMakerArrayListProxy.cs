using System;
using System.Collections;

public class PlayMakerArrayListProxy : PlayMakerCollectionProxy
{
	public ArrayList _arrayList;

	public ArrayList arrayList
	{
		get
		{
			return this._arrayList;
		}
	}

	public void Awake()
	{
		this._arrayList = new ArrayList();
		this.PreFillArrayList();
	}

	public bool isCollectionDefined()
	{
		return this.arrayList != null;
	}

	public void Add(object value, string type, bool silent = false)
	{
		this.arrayList.Add(value);
		if (!silent)
		{
			base.dispatchEvent(this.addEvent, value, type);
		}
	}

	public int AddRange(ICollection collection, string type)
	{
		this.arrayList.AddRange(collection);
		return this.arrayList.Count;
	}

	public void InspectorEdit(int index)
	{
		base.dispatchEvent(this.setEvent, index, "int");
	}

	public void Set(int index, object value, string type)
	{
		this.arrayList[index] = value;
		base.dispatchEvent(this.setEvent, index, "int");
	}

	public bool Remove(object value, string type, bool silent = false)
	{
		if (this.arrayList.Contains(value))
		{
			this.arrayList.Remove(value);
			if (!silent)
			{
				base.dispatchEvent(this.removeEvent, value, type);
			}
			return true;
		}
		return false;
	}

	private void PreFillArrayList()
	{
		switch (this.preFillType)
		{
		case PlayMakerCollectionProxy.VariableEnum.GameObject:
			this.arrayList.InsertRange(0, this.preFillGameObjectList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Int:
			this.arrayList.InsertRange(0, this.preFillIntList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Float:
			this.arrayList.InsertRange(0, this.preFillFloatList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.String:
			this.arrayList.InsertRange(0, this.preFillStringList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Bool:
			this.arrayList.InsertRange(0, this.preFillBoolList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Vector3:
			this.arrayList.InsertRange(0, this.preFillVector3List);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Rect:
			this.arrayList.InsertRange(0, this.preFillRectList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Quaternion:
			this.arrayList.InsertRange(0, this.preFillQuaternionList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Color:
			this.arrayList.InsertRange(0, this.preFillColorList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Material:
			this.arrayList.InsertRange(0, this.preFillMaterialList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Texture:
			this.arrayList.InsertRange(0, this.preFillTextureList);
			break;
		case PlayMakerCollectionProxy.VariableEnum.Vector2:
			this.arrayList.InsertRange(0, this.preFillVector2List);
			break;
		case PlayMakerCollectionProxy.VariableEnum.AudioClip:
			this.arrayList.InsertRange(0, this.preFillAudioClipList);
			break;
		}
	}
}
