using System;
using UnityEngine;

public class LOD_SimpleToggleGO : MonoBehaviour
{
	public GameObject GO;

	public float VisibleDistance = 75f;

	private int wsToken = -1;

	private bool currentVisibility = true;

	private void Awake()
	{
	}

	private void Start()
	{
		this.RefreshVisibility(true);
	}

	private void OnDisable()
	{
		try
		{
			WorkScheduler.Unregister(new WorkScheduler.Task(this.RefreshVisibilityWork), this.wsToken);
			this.wsToken = -1;
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
		if (this.wsToken != -1)
		{
			WorkScheduler.Unregister(new WorkScheduler.Task(this.RefreshVisibilityWork), this.wsToken);
		}
		this.wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.RefreshVisibilityWork), base.transform.position, false);
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
		Vector3 position = base.transform.position;
		position.y = PlayerCamLocation.PlayerLoc.y;
		float num = (position - PlayerCamLocation.PlayerLoc).sqrMagnitude + (float)((!this.currentVisibility) ? 2 : -2);
		bool flag = num < this.VisibleDistance * this.VisibleDistance;
		if (flag != this.currentVisibility || force)
		{
			this.GO.SetActive(flag);
			this.currentVisibility = flag;
		}
	}
}
