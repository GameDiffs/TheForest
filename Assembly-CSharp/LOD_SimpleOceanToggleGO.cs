using System;
using TheForest.Utils;
using UnityEngine;

public class LOD_SimpleOceanToggleGO : MonoBehaviour
{
	public GameObject GO;

	private bool isRegisterd;

	private bool currentVisibility = true;

	private void Start()
	{
		this.RefreshVisibility(true);
	}

	private void OnDisable()
	{
		try
		{
			if (this.isRegisterd)
			{
				WorkScheduler.UnregisterGlobal(new WorkScheduler.Task(this.RefreshVisibilityWork));
				this.isRegisterd = false;
			}
		}
		catch
		{
		}
	}

	private void OnEnable()
	{
		if (!LevelSerializer.IsDeserializing)
		{
			this.WSRegistration();
		}
		else
		{
			base.Invoke("WSRegistration", 0.005f);
		}
	}

	private void WSRegistration()
	{
		if (this.isRegisterd)
		{
			WorkScheduler.UnregisterGlobal(new WorkScheduler.Task(this.RefreshVisibilityWork));
		}
		WorkScheduler.RegisterGlobal(new WorkScheduler.Task(this.RefreshVisibilityWork), false);
		this.isRegisterd = true;
		this.RefreshVisibility(true);
	}

	private void RefreshVisibilityWork()
	{
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			this.RefreshVisibility(false);
		}
	}

	private void RefreshVisibility(bool force)
	{
		if (LocalPlayer.GameObject)
		{
			bool flag = LocalPlayer.Buoyancy.InWater && LocalPlayer.Buoyancy.IsOcean;
			if (flag != this.currentVisibility || force)
			{
				this.GO.SetActive(flag);
				this.currentVisibility = flag;
			}
		}
	}
}
