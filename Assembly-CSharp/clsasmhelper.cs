using System;
using U_r_g_utils;
using UnityEngine;

public class clsasmhelper : MonoBehaviour
{
	public Rigidbody varragdollbody;

	public LayerMask vargamlayermask;

	private bool vartriggered;

	private clsurganimationstatesmanager varasm;

	private void Start()
	{
		this.varasm = base.GetComponent<clsurganimationstatesmanager>();
		if (this.varasm == null)
		{
			base.enabled = false;
		}
	}

	private void metgetup()
	{
		if (this.vartriggered && this.varragdollbody.velocity.sqrMagnitude < 3f)
		{
			Transform transform = this.varragdollbody.transform;
			RaycastHit raycastHit;
			if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 1f, this.vargamlayermask))
			{
				base.GetComponent<Animation>()["get_up"].wrapMode = WrapMode.ClampForever;
				clsurgutils.metcrossfadetransitionanimation(transform, "get_up", 1f, base.transform, "get_up", true, "transition", null, null, null);
			}
			else
			{
				base.GetComponent<Animation>()["get_up_back"].wrapMode = WrapMode.ClampForever;
				clsurgutils.metcrossfadetransitionanimation(transform, "get_up_back", 1f, base.transform, "get_up_back", true, "transition", null, null, null);
			}
			this.vartriggered = false;
			base.CancelInvoke("metgetup");
		}
	}

	private void OnTriggerEnter(Collider varpother)
	{
		if (!this.vartriggered && varpother.CompareTag("missile"))
		{
			this.vartriggered = true;
			base.InvokeRepeating("metgetup", 4f, 4f);
		}
	}
}
