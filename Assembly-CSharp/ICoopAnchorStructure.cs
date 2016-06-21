using System;
using System.Collections.Generic;
using TheForest.Buildings.Creation;

public interface ICoopAnchorStructure
{
	List<StructureAnchor> Anchors
	{
		get;
		set;
	}

	int GetAnchorIndex(StructureAnchor anchor);

	StructureAnchor GetAnchor(int anchor);
}
