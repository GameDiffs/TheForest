using System;
using TheForest.UI;

public class SheenBillboardSlave : SheenBillboard
{
	public override void OnEnable()
	{
		if (this._action != InputMappingIcons.Actions.None)
		{
			ActionIcon actionIcon = ActionIconSystem.GetActionIcon(base.transform.parent);
			actionIcon._follow._target2 = base.transform;
		}
	}

	public override void OnDisable()
	{
	}
}
