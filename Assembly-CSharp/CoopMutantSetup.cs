using Bolt;
using System;
using UnityEngine;

public class CoopMutantSetup : EntityEventListener<IMutantState>
{
	public GameObject BloodPos;

	public GameObject BloodSplat;

	public Transform nooseTrapPivot;

	public GameObject nooseTrapGo;

	public clientNooseTrapFixer nooseFixer;

	public override void OnEvent(FxEnemeyBlood evnt)
	{
		this.BloodActual();
	}

	private void setClientNoosePivot(Transform tr)
	{
		this.nooseTrapPivot = tr;
	}

	private void setClientTrigger(GameObject go)
	{
		this.nooseTrapGo = go;
		this.nooseFixer = go.GetComponent<clientNooseTrapFixer>();
	}

	private void BloodActual()
	{
		if (this.BloodSplat && this.BloodPos)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.BloodSplat, this.BloodPos.transform.position, this.BloodPos.transform.rotation) as GameObject;
			gameObject.transform.parent = this.BloodPos.transform;
			UnityEngine.Object.Destroy(gameObject, 0.5f);
		}
	}
}
