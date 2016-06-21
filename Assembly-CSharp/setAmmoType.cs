using System;
using UnityEngine;

public class setAmmoType : MonoBehaviour
{
	public int ammoType;

	public GameObject headGo;

	public Renderer rockRender;

	private void OnDeserialized()
	{
		if (base.GetComponent<EmptyObjectIdentifier>())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			base.Invoke("setupFromLoad", 1f);
		}
	}

	private void setupFromLoad()
	{
		this.setupAmmoType(this.ammoType);
	}

	public void setupAmmoType(int type)
	{
		this.ammoType = type;
		if (this.ammoType == 0)
		{
			this.rockRender.enabled = true;
			this.headGo.SetActive(false);
		}
		if (this.ammoType == 1)
		{
			this.rockRender.enabled = false;
			this.headGo.SetActive(true);
		}
	}
}
