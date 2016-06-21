using System;
using UnityEngine;

public class LOD_BillboardOnly : MonoBehaviour
{
	public GameObject billboard;

	private CustomBillboard cb;

	private int currentLOD = -1;

	private int BillboardId = -1;

	private void OnEnable()
	{
		if (this.cb == null)
		{
			if (this.billboard != null)
			{
				this.cb = this.billboard.GetComponent<CustomBillboard>();
			}
			if (this.BillboardId == -1 && this.cb != null)
			{
				this.BillboardId = this.cb.Register(base.transform.position, base.transform.eulerAngles.y);
			}
		}
		else
		{
			this.cb.SetAlive(this.BillboardId, true);
		}
	}

	private void OnDisable()
	{
		if (this.BillboardId >= 0 && this.cb != null)
		{
			this.cb.SetAlive(this.BillboardId, false);
		}
	}
}
