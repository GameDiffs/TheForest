using System;
using UnityEngine;

public class mutantTargetSwitching : MonoBehaviour
{
	private mutantScriptSetup setup;

	public bool typeFatCreepy;

	public bool regular;

	public int attackerType;

	public GameObject currentAttackerGo;

	private void Awake()
	{
		this.setup = base.transform.root.GetComponentInChildren<mutantScriptSetup>();
	}

	public void getAttackerType(int type)
	{
		this.attackerType = type;
	}

	private void getAttacker(GameObject go)
	{
		this.currentAttackerGo = go;
		if (this.attackerType == 0)
		{
			if (this.setup.ai.maleSkinny || this.setup.ai.femaleSkinny)
			{
				this.setup.search.switchToNewTarget(go);
			}
			else if (!this.setup.ai.male && !this.setup.ai.female)
			{
				this.setup.search.switchToNewTarget(go);
			}
		}
		if (this.attackerType == 1)
		{
			this.setup.search.switchToNewTarget(go);
		}
		if (this.attackerType == 2 && !this.setup.ai.pale)
		{
			this.setup.search.switchToNewTarget(go);
		}
		if (this.attackerType == 3)
		{
			this.setup.search.switchToNewTarget(go);
		}
		if (this.attackerType == 4)
		{
			this.setup.search.switchToNewTarget(go);
		}
	}
}
