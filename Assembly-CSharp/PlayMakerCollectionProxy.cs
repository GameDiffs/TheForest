using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayMakerCollectionProxy : MonoBehaviour
{
	public enum VariableEnum
	{
		GameObject,
		Int,
		Float,
		String,
		Bool,
		Vector3,
		Rect,
		Quaternion,
		Color,
		Material,
		Texture,
		Vector2,
		AudioClip
	}

	public bool showEvents;

	public bool showContent;

	public bool TextureElementSmall;

	public bool condensedView;

	public bool liveUpdate;

	public string referenceName = string.Empty;

	public bool enablePlayMakerEvents;

	public string addEvent;

	public string setEvent;

	public string removeEvent;

	public int contentPreviewStartIndex;

	public int contentPreviewMaxRows = 10;

	public PlayMakerCollectionProxy.VariableEnum preFillType;

	public int preFillObjectTypeIndex;

	public int preFillCount;

	public List<string> preFillKeyList = new List<string>();

	public List<bool> preFillBoolList = new List<bool>();

	public List<Color> preFillColorList = new List<Color>();

	public List<float> preFillFloatList = new List<float>();

	public List<GameObject> preFillGameObjectList = new List<GameObject>();

	public List<int> preFillIntList = new List<int>();

	public List<Material> preFillMaterialList = new List<Material>();

	public List<UnityEngine.Object> preFillObjectList = new List<UnityEngine.Object>();

	public List<Quaternion> preFillQuaternionList = new List<Quaternion>();

	public List<Rect> preFillRectList = new List<Rect>();

	public List<string> preFillStringList = new List<string>();

	public List<Texture2D> preFillTextureList = new List<Texture2D>();

	public List<Vector2> preFillVector2List = new List<Vector2>();

	public List<Vector3> preFillVector3List = new List<Vector3>();

	public List<AudioClip> preFillAudioClipList = new List<AudioClip>();

	internal string getFsmVariableType(VariableType _type)
	{
		return _type.ToString();
	}

	internal void dispatchEvent(string anEvent, object value, string type)
	{
		if (!this.enablePlayMakerEvents)
		{
			return;
		}
		switch (type)
		{
		case "bool":
			Fsm.EventData.BoolData = (bool)value;
			break;
		case "color":
			Fsm.EventData.ColorData = (Color)value;
			break;
		case "float":
			Fsm.EventData.FloatData = (float)value;
			break;
		case "gameObject":
			Fsm.EventData.ObjectData = (GameObject)value;
			break;
		case "int":
			Fsm.EventData.IntData = (int)value;
			break;
		case "material":
			Fsm.EventData.MaterialData = (Material)value;
			break;
		case "object":
			Fsm.EventData.ObjectData = (UnityEngine.Object)value;
			break;
		case "quaternion":
			Fsm.EventData.QuaternionData = (Quaternion)value;
			break;
		case "rect":
			Fsm.EventData.RectData = (Rect)value;
			break;
		case "string":
			Fsm.EventData.StringData = (string)value;
			break;
		case "texture":
			Fsm.EventData.TextureData = (Texture)value;
			break;
		case "vector2":
			Fsm.EventData.Vector3Data = (Vector3)value;
			break;
		case "vector3":
			Fsm.EventData.Vector3Data = (Vector3)value;
			break;
		}
		FsmEventTarget fsmEventTarget = new FsmEventTarget();
		fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
		List<Fsm> list = new List<Fsm>(Fsm.FsmList);
		if (list.Count > 0)
		{
			Fsm fsm = list[0];
			fsm.Event(fsmEventTarget, anEvent);
		}
	}

	public void cleanPrefilledLists()
	{
		if (this.preFillBoolList.Count > this.preFillCount)
		{
			this.preFillBoolList.RemoveRange(this.preFillCount, this.preFillBoolList.Count - this.preFillCount);
		}
		if (this.preFillColorList.Count > this.preFillCount)
		{
			this.preFillColorList.RemoveRange(this.preFillCount, this.preFillColorList.Count - this.preFillCount);
		}
		if (this.preFillFloatList.Count > this.preFillCount)
		{
			this.preFillFloatList.RemoveRange(this.preFillCount, this.preFillFloatList.Count - this.preFillCount);
		}
		if (this.preFillIntList.Count > this.preFillCount)
		{
			this.preFillIntList.RemoveRange(this.preFillCount, this.preFillIntList.Count - this.preFillCount);
		}
		if (this.preFillMaterialList.Count > this.preFillCount)
		{
			this.preFillMaterialList.RemoveRange(this.preFillCount, this.preFillMaterialList.Count - this.preFillCount);
		}
		if (this.preFillObjectList.Count > this.preFillCount)
		{
			this.preFillObjectList.RemoveRange(this.preFillCount, this.preFillObjectList.Count - this.preFillCount);
		}
		if (this.preFillQuaternionList.Count > this.preFillCount)
		{
			this.preFillQuaternionList.RemoveRange(this.preFillCount, this.preFillQuaternionList.Count - this.preFillCount);
		}
		if (this.preFillRectList.Count > this.preFillCount)
		{
			this.preFillRectList.RemoveRange(this.preFillCount, this.preFillRectList.Count - this.preFillCount);
		}
		if (this.preFillStringList.Count > this.preFillCount)
		{
			this.preFillStringList.RemoveRange(this.preFillCount, this.preFillStringList.Count - this.preFillCount);
		}
		if (this.preFillTextureList.Count > this.preFillCount)
		{
			this.preFillTextureList.RemoveRange(this.preFillCount, this.preFillTextureList.Count - this.preFillCount);
		}
		if (this.preFillVector2List.Count > this.preFillCount)
		{
			this.preFillVector2List.RemoveRange(this.preFillCount, this.preFillVector2List.Count - this.preFillCount);
		}
		if (this.preFillVector3List.Count > this.preFillCount)
		{
			this.preFillVector3List.RemoveRange(this.preFillCount, this.preFillVector3List.Count - this.preFillCount);
		}
		if (this.preFillAudioClipList.Count > this.preFillCount)
		{
			this.preFillAudioClipList.RemoveRange(this.preFillCount, this.preFillAudioClipList.Count - this.preFillCount);
		}
	}
}
