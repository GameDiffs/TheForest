using System;
using System.Collections.Generic;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

[DoNotSerializePublic]
public class GreebleZonesManager : MonoBehaviour
{
	[Serializable]
	public class GZData
	{
		public int _seed = -1;

		public byte[] _instancesState;

		[DoNotSerialize]
		public GreebleZone GZ
		{
			get;
			set;
		}

		[DoNotSerialize]
		public int GZMid
		{
			get;
			set;
		}
	}

	public const byte ZIS_DestroyedInstance = 255;

	public const byte ZIS_UnusedInstanceSlot = 254;

	public const byte ZIS_ActiveInstance = 253;

	public const byte ZIS_CustomActiveInstance = 252;

	public int _zoneCount;

	[SerializeThis]
	public GreebleZonesManager.GZData[] _greebleZonesData;

	public GreebleZone[] _greebleZones;

	public int _currentVersion = 1;

	[SerializeThis]
	private int _savedVersion;

	private void Awake()
	{
		if (!LevelSerializer.IsDeserializing)
		{
			for (int i = 0; i < this._zoneCount; i++)
			{
				GreebleZone greebleZone = this._greebleZones[i];
				if (greebleZone)
				{
					greebleZone.Data = this._greebleZonesData[i];
					greebleZone.Data.GZ = greebleZone;
					greebleZone.Data.GZMid = i;
				}
			}
		}
	}

	public void RefreshGreebleZones()
	{
		if (!Application.isPlaying)
		{
			this._currentVersion++;
		}
		this._greebleZones = UnityEngine.Object.FindObjectsOfType<GreebleZone>();
		this._zoneCount = this._greebleZones.Length;
		this._greebleZonesData = new GreebleZonesManager.GZData[this._greebleZones.Length];
		for (int i = 0; i < this._greebleZones.Length; i++)
		{
			GreebleZonesManager.GZData gZData = this._greebleZonesData[i] = new GreebleZonesManager.GZData();
			gZData._instancesState = new byte[this._greebleZones[i].MaxInstances];
			for (int j = 0; j < gZData._instancesState.Length; j++)
			{
				gZData._instancesState[j] = 254;
			}
			gZData._seed = -1;
			if (Application.isPlaying)
			{
				this._greebleZones[i].Data = gZData;
				gZData.GZ = this._greebleZones[i];
				gZData.GZMid = i;
			}
		}
	}

	private void OnSerializing()
	{
		int num = Mathf.Min(new int[]
		{
			this._zoneCount,
			this._greebleZones.Length,
			this._greebleZonesData.Length
		});
		for (int i = 0; i < num; i++)
		{
			GreebleZone greebleZone = this._greebleZones[i];
			GreebleZonesManager.GZData gZData = this._greebleZonesData[i];
			if (greebleZone && greebleZone.CurrentlyVisible && gZData != null && gZData._instancesState != null && gZData._instancesState.Length == greebleZone.Instances.Length)
			{
				int j;
				for (j = 0; j < greebleZone.Instances.Length; j++)
				{
					if (greebleZone.Instances[j])
					{
						if (gZData._instancesState[j] > 252)
						{
							gZData._instancesState[j] = 253;
						}
					}
					else
					{
						gZData._instancesState[j] = 255;
					}
				}
				while (j < this._greebleZones[i].MaxInstances)
				{
					gZData._instancesState[j] = 254;
					j++;
				}
			}
			else if (greebleZone && gZData == null)
			{
				gZData = (this._greebleZonesData[i] = new GreebleZonesManager.GZData());
				gZData._instancesState = new byte[greebleZone.MaxInstances];
				for (int k = 0; k < gZData._instancesState.Length; k++)
				{
					gZData._instancesState[k] = 254;
				}
			}
		}
		this._savedVersion = this._currentVersion;
	}

	private void OnDeserialized()
	{
		if (this._savedVersion == this._currentVersion)
		{
			if (this._greebleZonesData.Length != this._zoneCount)
			{
				if (this._greebleZonesData.Length <= this._zoneCount)
				{
					this.RefreshGreebleZones();
					return;
				}
				List<GreebleZonesManager.GZData> list = this._greebleZonesData.ToList<GreebleZonesManager.GZData>();
				list.RemoveRange(this._zoneCount, this._greebleZonesData.Length - this._zoneCount);
				this._greebleZonesData = list.ToArray();
			}
			for (int i = 0; i < this._zoneCount; i++)
			{
				GreebleZonesManager.GZData gZData = this._greebleZonesData[i];
				if (gZData == null)
				{
					gZData = (this._greebleZonesData[i] = new GreebleZonesManager.GZData());
					gZData._instancesState = new byte[this._greebleZones[i].MaxInstances];
					for (int j = 0; j < gZData._instancesState.Length; j++)
					{
						gZData._instancesState[j] = 254;
					}
				}
				else
				{
					GreebleZone greebleZone = this._greebleZones[i];
					if (greebleZone)
					{
						if (gZData._instancesState.Length != greebleZone.MaxInstances)
						{
							gZData._instancesState = new byte[greebleZone.MaxInstances];
							for (int k = 0; k < gZData._instancesState.Length; k++)
							{
								gZData._instancesState[k] = 254;
							}
						}
						greebleZone.Data = gZData;
						gZData.GZ = greebleZone;
						gZData.GZMid = i;
					}
				}
			}
		}
		else if (!Scene.Atmosphere.InACave)
		{
			Debug.Log("Clearing up deprecated saved greeble data");
			this.RefreshGreebleZones();
		}
		else
		{
			Debug.Log("Greeble data out of sync. Restoring not handled in caves.");
		}
	}
}
